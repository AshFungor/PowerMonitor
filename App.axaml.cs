using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ExtremelySimpleLogger;
using GemBox.Spreadsheet;
using PowerMonitor.models;
using PowerMonitor.services;
using SPath = PowerMonitor.services.SettingsService.Settings;

namespace PowerMonitor;

public static class Shared
{
    public static MainWindow? MainWin = null;
    public static Plot? Plot;
    public static Logger Logger = new() {Sinks = {new FileSink(SPath.DataFolder + "monitor.log", true)}};
}

public class App : Application
{
    public static string Path
#if LINUX
        => $"/home/{Environment.UserName}/Documents/";
#elif WINDOWS
        => $"C:/Users/{Environment.UserName}/Documents/";
#endif


    public override void Initialize()
    {
        // method loads twice, careful adding init
        AvaloniaXamlLoader.Load(this);
        File.WriteAllText(SPath.DataFolder + "monitor.log", string.Empty);

        Shared.Logger.Log(LogLevel.Info, $"beginning new session on {DateTime.Now}");

        LoginService.InitLoginService();

        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }
}