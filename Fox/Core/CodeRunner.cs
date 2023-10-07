using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using Esprima;
using Fox.Core.Drawing;
using Fox.Models;
using Jint;
using Jint.Runtime;

namespace Fox.Core
{
    public class CodeRunner
    {
        Color bgColor;
        Color fgColor = Colors.Black;

        double thickness = 10;
        bool hasStroke = false;

        DrawCommandRegister drawCommands;
        ObservableCollection<Log> logs;

        CancellationTokenSource cancellationToken;

        public CodeRunner(DrawCommandRegister drawCommands, ObservableCollection<Log> logs, Color bgColor)
        {
            this.drawCommands = drawCommands;
            this.logs = logs;
            this.bgColor = bgColor;
        }

        public Task Run(string text, CancellationTokenSource token)
        {
            cancellationToken = token;
            return Task.Run(() => 
            {
                Engine engine = new Engine(options => 
                {
                    options.Strict = true;
                    options.CancellationToken(token.Token); 
                    options.Constraints.Constraints.Add(new SleepConstraint(10));
                })
                .SetValue("width",     GetWidth)
                .SetValue("height",    GetHeight)
                .SetValue("rand",      Rand)
                .SetValue("randint",   RandInt)
                .SetValue("wait",      Wait)
                .SetValue("clear",     ClearBg)
                .SetValue("thickness", SetThickness)
                .SetValue("stroke",    SetStroke)
                .SetValue("color",     SetColor)
                .SetValue("line",      DrawLine)
                .SetValue("circle",    DrawCircle)
                .SetValue("rect",      DrawRect)
                .SetValue("log",       LogInfo)
                .SetValue("warn",      LogWarning)
                .SetValue("error",     LogError);

                ClearBg(bgColor.R, bgColor.G, bgColor.B);

                try
                {
                    engine.Execute(text);
                }
                catch (ExecutionCanceledException)
                {

                }
                catch (JavaScriptException e)
                {
                    logs.Add(Logger.Error($"(runtime) {e.Message} at line {e.LineNumber}"));
                    token.Cancel();
                }
                catch (ParserException e)
                {
                    var msg = e.Message.Substring(e.Message.IndexOf(':') + 2);
                    logs.Add(Logger.Error($"(parser)  {msg} at line {e.LineNumber}"));
                    token.Cancel();
                }
#if !DEBUG
                catch (Exception e)
                {
                    logs.Add(Logger.Error($"(internal) {e.Message}"));
                    token.Cancel();
                }
#endif
            }, token.Token);
        }

        int GetWidth() => 1000;
        int GetHeight() => 1000;

        void Wait(int milliSeconds) 
        {
            try
            {
                Task.Delay(milliSeconds).Wait(cancellationToken.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        double Rand(double start, double end) 
        {
            return start + Random.Shared.NextDouble() * (end - start);
        }

        int RandInt(int start, int end)
        {
            return Random.Shared.Next(start, end);
        }

        void SetColor(int a, int r, int g, int b) 
        {
            JsExeptions.IsColorValid(a, r, g, b);
            fgColor = new Color((byte)a, (byte)r, (byte)g, (byte)b);
        }

        void SetThickness(double value)
        {
            thickness = value;
        }

        void SetStroke(bool value) 
        {
            hasStroke = value;
        }

        void DrawLine(double x, double y, double x2, double y2) 
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                drawCommands.Add((d) =>
                {
                    d.DrawLine(
                        new Pen(new SolidColorBrush(fgColor), thickness),
                        new Point(x, y),
                        new Point(x2, y2));
                });
            });
        }

        void DrawCircle(double x, double y, double radius)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                drawCommands.Add((d) =>
                {
                    d.DrawEllipse(
                            hasStroke ? null : new SolidColorBrush(fgColor),
                            hasStroke ? new Pen(new SolidColorBrush(fgColor), thickness) : null,
                            new Point(x, y),
                            radius,
                            radius);
                });
            });
        }

        void DrawRect(double x, double y, double width, double height) 
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                drawCommands.Add((d) =>
                {
                    d.DrawRectangle(
                            hasStroke ? null : new SolidColorBrush(fgColor),
                            hasStroke ? new Pen(new SolidColorBrush(fgColor), thickness) : null,
                            new Rect(x, y, width, height));
                });
            });
        }
        
        void ClearBg(int r, int g, int b)
        {
            JsExeptions.IsColorValid(255, r, g, b);

            Dispatcher.UIThread.Invoke(() =>
            {
                drawCommands.Add((d) =>
                {
                    d.DrawRectangle(
                        new SolidColorBrush(new Color(255, (byte)r, (byte)g, (byte)b)),
                        null,
                        new Rect(0, 0, 1000, 1000));
                });
            });
        }

        void LogInfo(string message)
        {
            logs.Add(Logger.Info(message));
        }

        void LogWarning(string message)
        {
            logs.Add(Logger.Warn(message));
        }

        void LogError(string message)
        {
            logs.Add(Logger.Error(message));
        }
    }
}
