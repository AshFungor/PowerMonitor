using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.models;

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