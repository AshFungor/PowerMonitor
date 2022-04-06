using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExtremelySimpleLogger;
using PowerMonitor.services;
using SPath = PowerMonitor.services.SettingsService.Settings;

namespace PowerMonitor.models;

public class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Content = new Login();
        Shared.MainWin = this;
        Closed += OnClose;
        Width = MinWidth;
        Height = MinHeight;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnClose(object? sender, EventArgs args)
    {
        Shared.Logger!.Log(LogLevel.Info, "closing main window");
        LoginService.UpdateLogins();
        SPath.Save();
        Thread.Sleep(100);
    }
}