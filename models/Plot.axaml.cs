using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OxyPlot;
using OxyPlot.Avalonia;
using SimpleLogger;

namespace PowerMonitor.models;

public class Plot : UserControl
{
    private OxyPlot.Avalonia.Plot _plot;

    public Plot()
    {
        InitializeComponent();
        Logger.Log<Plot>("beginning building plot...");
        
        
        _plot = this.FindControl<OxyPlot.Avalonia.Plot>("Plot");
        _plot.Title = "Plot";
        var start = new DataPoint(0, 0);
        _plot.Annotations.Add(new ArrowAnnotation()
            {StartPoint = start, EndPoint = new DataPoint(0, 100), Color = new Color(255, 0, 0, 0)});
        _plot.Annotations.Add(new ArrowAnnotation()
            {StartPoint = start, EndPoint = new DataPoint(24, 0), Color = new Color(255, 0, 0, 0)});
        Shared.Plot ??= this;


#if !SERVER && DEBUG
        var rand = new Random();
        var line = new OxyPlot.Series.LineSeries();
        for (var i = 0; i < 24; ++i)
            line.Points.Add(new DataPoint(i, rand.Next(0, 100)));
        _plot.ActualModel.Series.Add(line);
        _plot.InvalidatePlot();
#endif
    }

    public void AddSeries(List<(double, double)> source)
    {
        Logger.Log<Plot>($"constructing series, source len is {source.Count}");
        var line = new OxyPlot.Series.LineSeries();
        foreach (var dot in source) line.Points.Add(new DataPoint(dot.Item2, dot.Item1));
        _plot.ActualModel.Series.Clear();
        _plot.ActualModel.Series.Add(line);
        
        
        _plot.InvalidatePlot();
        Logger.Log<Plot>("rendering plot");
    }

    public void ShapeLowAnnotation(int newLimit)
    {
        Logger.Log<Plot>($"reshaping plot x asis to {newLimit}");
        
        
        _plot.Axes[0].Maximum = newLimit;
        _plot.Annotations.RemoveAt(1);
        _plot.Annotations.Add(new ArrowAnnotation()
            {StartPoint = new DataPoint(0, 0), EndPoint = new DataPoint(newLimit, 0), Color = new Color(255, 0, 0, 0)});
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}