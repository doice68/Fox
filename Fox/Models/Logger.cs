using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Fox.Models
{
    public class Log
    {
        public IBrush Color { get; set; }
        public string Message { get; set; }

        public Log(IBrush color, string message)
        {
            this.Color = color;
            this.Message = message;
        }
    }

    public static class Logger
    {
        static ImmutableSolidColorBrush purple = new ImmutableSolidColorBrush(new Color(255, 157, 105, 243));
        static ImmutableSolidColorBrush yellow = new ImmutableSolidColorBrush(new Color(255, 242, 174, 104));
        static ImmutableSolidColorBrush red = new ImmutableSolidColorBrush(new Color(255, 245, 88, 78));

        public static Log FoxLog(string msg)
        {
            return new Log(purple, "FOX: " + msg);
        }

        public static Log Info(string msg)
        {
            return new Log(Brushes.White, msg);
        }

        public static Log Warn(string msg) 
        {
            return new Log(yellow, "WARN: " + msg); 
        }

        public static Log Error(string msg)
        {
            return new Log(red, "ERROR: " + msg);
        }
    }

}
