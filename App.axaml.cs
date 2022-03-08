using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using PowerMonitor.controllers;
using PowerMonitor.models;
using ExtremelySimpleLogger;
using GemBox.Spreadsheet;


namespace PowerMonitor;

public static class Shared
{
    public static LoginController? LoginController;
    public static DataController? DataController;
    public static MainWindow? MainWin = null;
    public static Plot? Plot;
    public static Logger? Logger;
}

public class App : Application
{
    public static string SettingsPath
#if LINUX && !DEBUG
        => $"/home/{Environment.UserName}/.config/PowerMonitor";
#elif LINUX && DEBUG
        // change path to your local resources
        => $"/home/{Environment.UserName}/Documents/";
#elif WINDOWS && DEBUG
        => $"C:/Users/Environment.UserName}/Documents/";
#else
        => string.Empty;
#endif


    public override void Initialize()
    {
        // method loads twice, careful adding init
        AvaloniaXamlLoader.Load(this);
        File.WriteAllText(SettingsPath + "monitor.log", string.Empty);

        Shared.Logger = new Logger()
            {Sinks = {new FileSink(SettingsPath + "monitor.log", true)}};
        Shared.LoginController ??= new LoginController();
        Shared.DataController ??= new DataController();

        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }
}