using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.models;

public class UserView : UserControl
{
    private UserControl _plotControl;
    protected TabControl _tabControl;

    public UserView()
    {
        InitializeComponent();
        _plotControl = this.Find<UserControl>("PlotItem");
        _plotControl.Content = new Plot();
        _tabControl = this.Find<TabControl>("TabControl");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}