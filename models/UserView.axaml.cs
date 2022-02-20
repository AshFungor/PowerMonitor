using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.models;

public class UserView : UserControl
{
    private UserControl _plotControl;
    protected readonly TabControl _tabControl;
    private ListBox _targetDevListBox;

    public UserView()
    {
        InitializeComponent();
        _plotControl = this.Find<UserControl>("PlotItem");
        _plotControl.Content = new Plot();
        _tabControl = this.Find<TabControl>("TabControl");
        _targetDevListBox = this.Find<ListBox>("TargetDevListBox");

        if (Shared.ConfigController.AppConfig.LocalPlotData)
        {
            List<ListBoxItem> list = new List<ListBoxItem>();
            list.Add(new ListBoxItem()
                {Content = "choose me"});
            list.Add(new ListBoxItem()
                {Content = "choose me"});
            list.Add(new ListBoxItem()
                {Content = "choose me"});
            list.Add(new ListBoxItem()
                {Content = "choose me"});
            _targetDevListBox.Items = list;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}