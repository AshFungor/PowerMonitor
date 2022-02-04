using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SimpleLogger;

namespace PowerMonitor.models
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Content = new Login();
            Shared.MainWin = this;
            Closed += OnClose;

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
            Logger.Log<MainWindow>("closing main window");
            Shared.LoginController.UpdateLogins();
        }
    }
}