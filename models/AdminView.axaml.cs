using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.views;

public class AdminView : UserControl
{
    public AdminView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}