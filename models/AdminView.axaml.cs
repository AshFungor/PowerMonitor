using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.models;

public class AdminView : UserControl
{
    private UserControl _plotControl;
    public AdminView()
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