using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ExtremelySimpleLogger;
using GemBox.Spreadsheet;
using PowerMonitor.controllers;
using PowerMonitor.models;
using SPath = PowerMonitor.controllers.SettingsController.Settings;

namespace PowerMonitor;

public static class Shared
{
    public static LoginController? LoginController;
    public static DataController? DataController;
    public static NetworkController? NetworkController;
    public static MainWindow? MainWin = null;
    public static Plot? Plot;
    public static Logger? Logger;
}

public class App : Application
{
    public static string SettingsPath
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

        Shared.Logger = new Logger {Sinks = {new FileSink(SPath.DataFolder + "monitor.log", true)}};
        Shared.Logger.Log(LogLevel.Info, $"beginning new session on {DateTime.Now}");
        Shared.LoginController ??= new LoginController();
        Shared.DataController ??= new DataController();
        Shared.NetworkController ??= new NetworkController();

        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }
}