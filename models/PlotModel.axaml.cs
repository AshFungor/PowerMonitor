using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ExtremelySimpleLogger;
using OxyPlot;
using OxyPlot.Avalonia;
using LineSeries = OxyPlot.Series.LineSeries;

namespace PowerMonitor.models;

public class Plot : UserControl
{
    private readonly OxyPlot.Avalonia.Plot _plot;

    public Plot()
    {
        InitializeComponent();
        Shared.Logger!.Log(LogLevel.Info, "beginning building plot...");
        

        _plot = this.FindControl<OxyPlot.Avalonia.Plot>("Plot");
        _plot.Title = "Plot";
        var start = new DataPoint(0, 0);
        _plot.Annotations.Add(new ArrowAnnotation
            {StartPoint = start, EndPoint = new DataPoint(0, 100), Color = new Color(255, 0, 0, 0)});
        _plot.Annotations.Add(new ArrowAnnotation
            {StartPoint = start, EndPoint = new DataPoint(24, 0), Color = new Color(255, 0, 0, 0)});
        Shared.Plot ??= this;
    }

    public void AddSeries(List<(double, double)> source)
    {
        Shared.Logger!.Log(LogLevel.Info, $"constructing series, source len is {source.Count}");
        var line = new LineSeries();
        foreach (var dot in source) line.Points.Add(new DataPoint(dot.Item2, dot.Item1));
        _plot.ActualModel.Series.Clear();
        _plot.ActualModel.Series.Add(line);

        
        _plot.InvalidatePlot();
        Shared.Logger!.Log(LogLevel.Info, "rendering plot");
    }

    public void ShapeLowAnnotation(int newLimit)
    {
        Shared.Logger!.Log(LogLevel.Info, $"reshaping plot x asis to {newLimit}");


        _plot.Axes[0].Maximum = newLimit;
        _plot.Annotations.RemoveAt(1);
        _plot.Annotations.Add(new ArrowAnnotation
            {StartPoint = new DataPoint(0, 0), EndPoint = new DataPoint(newLimit, 0), Color = new Color(255, 0, 0, 0)});
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}