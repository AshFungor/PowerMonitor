using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.models;

public class Plot : UserControl
{
    public Plot()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}