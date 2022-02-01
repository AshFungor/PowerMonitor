using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.views;

public class UserView : UserControl
{
    public UserView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}