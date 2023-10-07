using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Fox.Core;
using Fox.Core.Drawing;
using Fox.Models;
using Fox.Serivces;

namespace Fox.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    string DefaultCode =>
@"clear(35, 35, 35);
thickness(10);
stroke(true);
color(255, 255, 255, 255);

let radius = 50;

for (let i = 0; i < 10; i++)
{
	for (let k = 0; k < 10; k++)
	{
		circle(radius + 100 * k, radius + 100 * i, radius );
	}
}";
    
    [ObservableProperty]
    public TextDocument code;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RunIcon))]
    [NotifyPropertyChangedFor(nameof(RunText))]
    bool running = false;

    public Symbol RunIcon => 
        Running == false? Symbol.Play : Symbol.Stop;

    public string RunText => 
        Running == false? "Run" : "Stop";

    ISolidColorBrush DefaultBgColor => Brushes.White;

    [ObservableProperty]
    bool alwaysClearConsole = true;

    [ObservableProperty]
    ObservableCollection<Log> logs;

    [ObservableProperty]
    RenderTargetBitmap bitmap;

    [ObservableProperty]
    DrawCommandRegister drawCommands;

    CodeRunner codeRunner;
    Task seasion;
    CancellationTokenSource cts;

    FileDialogService fileDialogService;

    public MainViewModel()
    {
        fileDialogService = new();
        DrawCommands = new();
        Logs = new();
        Code = new(DefaultCode);
        codeRunner = new(drawCommands, logs, DefaultBgColor.Color);
    }

    [RelayCommand]
    public async Task ToggleRunning()
    {
        cts?.Cancel();

        if (Running == false)
        {
            Running = true;

            if (AlwaysClearConsole)
                ClearConsole();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Logs.Add(Logger.FoxLog("Session started!"));

            cts = new();
            await codeRunner.Run(Code.Text, cts);

            stopwatch.Stop();
            Logs.Add(Logger.FoxLog($"Session ended! took {stopwatch.ElapsedMilliseconds}ms"));
            Running = false;
            cts.Cancel();
        }
        //if (Running == true)
        //{
        //    cts.Cancel();
        //    Logs.Add(Logger.FoxLog("Seasion stoped."));
        //    Running = false;
        //}
    }

    public void ClearConsole() 
    {
        Logs.Clear();
    }

    [RelayCommand]
    public async void OpenFile() 
    {
        var path = await fileDialogService.OpenFileDialog("Choose a file");
        if (path != null)
        {
            var text = File.ReadAllText(path);
            Code = new TextDocument(text);
        }
    }

    [RelayCommand]
    public async void SaveFile()
    {
        var path = await fileDialogService.SaveFileDialog("Choose a folder");
        if (path != null)
        {
            File.WriteAllText(path + ".js", Code.Text);
        }
    }

    [RelayCommand]
    public async void SaveImage()
    {
        var path = await fileDialogService.SaveFileDialog("Choose a folder");
        if (path != null)
        {
            Bitmap.Save(path + ".png");
        }
    }

    [RelayCommand]
    public void ShowDocs() 
    {
    
    }
}
