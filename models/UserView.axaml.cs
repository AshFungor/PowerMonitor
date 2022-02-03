using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.models;

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