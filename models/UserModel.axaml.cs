using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExtremelySimpleLogger;
using PowerMonitor.services;

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
        var list = new List<ComboBoxItem>();
        LoginService.Complexes.ForEach(complex => list.Add(new ComboBoxItem {Content = complex}));
        _targetDevComboBox.Items = list;
    }

    private async void Call(object? sender, RoutedEventArgs args)
    {
        Shared.Logger!.Log(LogLevel.Info, "downloading data...");
        if (!Check())
        {
            _logLabel.Content = "Fill all data first";
            return;
        }

        if (_downloadingProcessOnline) return;

        _logLabel.Content = "preparing for download...";
        _downloadingProcessOnline = true;
        var res = NetworkService.GetData(_startDatePicker.SelectedDate.Value.DateTime,
                _endDatePicker.SelectedDate.Value.DateTime, LoginService.Complexes[_targetDevComboBox.SelectedIndex])
            .Result;

        if (!res) return;
        var data = await DataService.EvaluateDataAsync(_startDatePicker.SelectedDate.Value.DateTime);
        Shared.Plot!.AddSeries(data);
        _downloadingProcessOnline = false;
        _logLabel.Content = "download complete.";

        Shared.Logger!.Log(LogLevel.Info, "download complete.");
    }

    private bool Check()
    {
        if (_endDatePicker.SelectedDate is not null && _startDatePicker.SelectedDate is not null)
            if (_targetDevComboBox.SelectedItem != null)
                return true;

        return false;
    }

    private async void LoadToSpreadsheet(object? sender, RoutedEventArgs args)
    {
        Shared.Logger!.Log(LogLevel.Info, "sending data to spreadsheet...");

        if (!_downloadingProcessOnline)
        {
            if (DataService.CheckResponse())
            {
                await DataService.LoadIntoSpreadsheetAsync();
            }
            else
            {
                _logLabel.Content = "you must first specify time and device and then push \"get data\" button.";
                return;
            }
        }

        _logLabel.Content = "data uploaded. check your file.";
        Shared.Logger!.Log(LogLevel.Info, "data was sended to spreadsheet.");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}