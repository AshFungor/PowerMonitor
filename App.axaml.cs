#define DEBUG
#define LINUX


using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Logging;
using PowerMonitor.controllers;




namespace PowerMonitor
{
    public class App : Application
    {
        #if LINUX && !DEBUG
        public static string SettingsPath => $"/home/{Environment.UserName}/.config/PowerMonitor";
        #elif LINUX && DEBUG
        // change path to your local resources
        public static string SettingsPath => $"/home/{Environment.UserName}/Documents/";
        #endif
        public LoginController LoginController;


        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            LoginController = new LoginController();
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