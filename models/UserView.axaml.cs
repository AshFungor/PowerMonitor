using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.models;

public class UserView : UserControl
{
    private readonly UserControl _plotControl;
    protected readonly TabControl _tabControl;
    private readonly DatePicker _datePicker;
    private readonly ComboBox _targetDevComboBox;

    public UserView()
    {
        InitializeComponent();
        _plotControl = this.Find<UserControl>("PlotItem");
        _plotControl.Content = new Plot();
        _tabControl = this.Find<TabControl>("TabControl");
        _targetDevComboBox = this.Find<ComboBox>("TargetDevComboBox");
        _datePicker = this.Find<DatePicker>("DatePicker");

#if DEBUG && !SERVER
        var list = new List<ComboBoxItem>();
        list.Add(new ComboBoxItem()
            {Content = "choose me"});
        list.Add(new ComboBoxItem()
            {Content = "choose me"});
        list.Add(new ComboBoxItem()
            {Content = "choose me"});
        list.Add(new ComboBoxItem()
            {Content = "choose me"});
        _targetDevComboBox.Items = list;
#endif
    }

    private async void Call(object? sender, RoutedEventArgs args)
    {
        var data = await Shared.DataController!.ReadData(new DateTime(2020, 1, 1, 0, 0, 0));
        Shared.Plot!.AddSeries(data);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}