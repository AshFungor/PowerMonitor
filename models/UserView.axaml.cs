using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExtremelySimpleLogger;

namespace PowerMonitor.models;

public class UserView : UserControl
{
    private readonly DatePicker _endDatePicker;
    private readonly Label _logLabel;
    private readonly UserControl _plotControl;
    private readonly DatePicker _startDatePicker;
    protected readonly TabControl _tabControl;
    private readonly ComboBox _targetDevComboBox;
    private bool _downloadingProcessOnline;

    public UserView()
    {
        InitializeComponent();
        _plotControl = this.Find<UserControl>("PlotItem");
        _plotControl.Content = new Plot();
        _tabControl = this.Find<TabControl>("TabControl");
        _targetDevComboBox = this.Find<ComboBox>("TargetDevComboBox");
        _startDatePicker = this.Find<DatePicker>("StartDatePicker");
        _endDatePicker = this.Find<DatePicker>("EndDatePicker");
        _logLabel = this.Find<Label>("LogLabel");

#if DEBUG && !SERVER
        var list = new List<ComboBoxItem>();
        Shared.NetworkController!.Complexes.ForEach(complex => list.Add(new ComboBoxItem() {Content = complex}));
        _targetDevComboBox.Items = list;
#endif
    }

    private async void Call(object? sender, RoutedEventArgs args)
    {
        _logLabel.Content = "preparing for download...";
        _downloadingProcessOnline = true;
        var data = await Shared.DataController!.EvaluateDataAsync(new DateTime(2020, 1, 1, 0, 0, 0));
        Shared.Plot!.AddSeries(data);
        _downloadingProcessOnline = false;
        _logLabel.Content = "download complete.";
    }

    private async void LoadToSpreadsheet(object? sender, RoutedEventArgs args)
    {
        Shared.Logger!.Log(LogLevel.Info, "sending data to spreadsheet...");

        if (!_downloadingProcessOnline)
        {
            if (Shared.DataController!.CheckResponse())
            {
                await Shared.DataController!.LoadIntoSpreadsheetAsync();
            }
            else
            {
                _logLabel.Content = "you must first specify time and device and then push \"get data\" button.";
                return;
            }
        }

        _logLabel.Content = "data uploaded. check your file.";
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}