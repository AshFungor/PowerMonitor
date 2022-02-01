using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.views;

public class Login : UserControl
{
    public Login()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}