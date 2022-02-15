using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.models;

public class UserView : UserControl
{
    private UserControl _plotControl;
    public UserView()
    {
        InitializeComponent();
        _plotControl = this.Find<UserControl>("PlotItem");
        _plotControl.Content = new Plot();

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}