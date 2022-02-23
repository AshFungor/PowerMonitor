using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SimpleLogger;

namespace PowerMonitor.models;

public class UserView : UserControl
{
    private readonly UserControl _plotControl;
    protected readonly TabControl _tabControl;
    private readonly DatePicker _startDatePicker;
    private readonly DatePicker _endDatePicker;
    private readonly ComboBox _targetDevComboBox;
    private bool _downloadingProcessOnline = false;

    public UserView()
    {
        InitializeComponent();
        _plotControl = this.Find<UserControl>("PlotItem");
        _plotControl.Content = new Plot();
        _tabControl = this.Find<TabControl>("TabControl");
        _targetDevComboBox = this.Find<ComboBox>("TargetDevComboBox");
        _startDatePicker = this.Find<DatePicker>("StartDatePicker");
        _endDatePicker = this.Find<DatePicker>("EndDatePicker");

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
        _downloadingProcessOnline = true;
        var data = await Shared.DataController!.EvaluateDataAsync(new DateTime(2020, 1, 1, 0, 0, 0));
        Shared.Plot!.AddSeries(data);
        _downloadingProcessOnline = false;
    }

    private async void LoadToSpreadsheet(object? sender, RoutedEventArgs args)
    {
        Logger.Log<UserView>("sending data to spreadsheet...");
        
        if (!_downloadingProcessOnline)
        {
            await Shared.DataController!.LoadIntoSpreadsheetAsync();
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}