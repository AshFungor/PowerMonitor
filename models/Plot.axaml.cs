using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OxyPlot;
using OxyPlot.Avalonia;
using OxyPlot.Series;
using LineSeries = OxyPlot.Avalonia.LineSeries;

namespace PowerMonitor.models;

public class Plot : UserControl
{
    private OxyPlot.Avalonia.Plot _plot;

    public Plot()
    {
        InitializeComponent();
        _plot = this.FindControl<OxyPlot.Avalonia.Plot>("Plot");
        _plot.Title = "Plot";
        var start = new DataPoint(0, 0);
        _plot.Annotations.Add(new ArrowAnnotation()
        {
            StartPoint = start,
            EndPoint = new DataPoint(0, 100),
            Color = new Color(255, 0, 0, 0)
        });
        _plot.Annotations.Add(new ArrowAnnotation()
        {
            StartPoint = start,
            EndPoint = new DataPoint(24, 0),
            Color = new Color(255, 0, 0, 0)
        });


        if (Shared.ConfigController.AppConfig.LocalPlotData)
        {
            var rand = new Random();
            var line = new OxyPlot.Series.LineSeries();
            for (var i = 0; i < 24; ++i)
                line.Points.Add(new DataPoint(i, rand.Next(0, 100)));
            _plot.ActualModel.Series.Add(line);
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}