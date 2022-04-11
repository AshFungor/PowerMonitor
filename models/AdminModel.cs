using Avalonia.Controls;

namespace PowerMonitor.models;

public class AdminView : UserView
{
    public AdminView()
    {
        var enumerator = _tabControl.Items.GetEnumerator();
        // so nice...
        enumerator.MoveNext();
        enumerator.MoveNext();
        var adminTab = enumerator.Current as TabItem;
        adminTab!.IsEnabled = true;
        adminTab!.Content = new AdminSettingsTab();
    }
}