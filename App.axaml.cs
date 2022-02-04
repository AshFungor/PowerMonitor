#define DEBUG
#define LINUX


using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.IO;
using PowerMonitor.controllers;
using PowerMonitor.models;
using SimpleLogger.Logging.Handlers;
using Logger = SimpleLogger.Logger;


namespace PowerMonitor
{
    public class Shared
    {
        public static LoginController? LoginController;
        public static DbController? DbController;
        public static MainWindow? MainWin = null;
    }
    public class App : Application
    {
        #if LINUX && !DEBUG
        public static string SettingsPath => $"/home/{Environment.UserName}/.config/PowerMonitor";
        #elif LINUX && DEBUG
        // change path to your local resources
        public static string SettingsPath => $"/home/{Environment.UserName}/Documents/";
        #endif
        


        public override void Initialize()
        {
            // method loads twice, careful adding init
            AvaloniaXamlLoader.Load(this);
            File.WriteAllText(SettingsPath + "monitor.log", String.Empty);
            Logger.LoggerHandlerManager.AddHandler(new FileLoggerHandler("monitor.log", App.SettingsPath));
            
            Shared.LoginController ??= new LoginController();
            Shared.DbController ??= new DbController();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}