using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PowerMonitor.views;

namespace PowerMonitor.models;

public class AdminView : UserView
{
    public AdminView() : base()
    {
        var enumerator = _tabControl.Items.GetEnumerator();

        enumerator.MoveNext();
        enumerator.MoveNext();
        var adminTab = (TabItem) enumerator.Current;
        adminTab.IsEnabled = true;
        adminTab.Content = new AdminSettingsTab();
    }
}