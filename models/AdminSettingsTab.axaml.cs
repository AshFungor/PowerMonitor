using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.views;

public partial class AdminSettingsTab : UserControl
{
    private ListBox _listBox;
    public AdminSettingsTab()
    {
        InitializeComponent();
        _listBox = this.Find<ListBox>("UserInfoListBox");
        _listBox.Items = Shared.LoginController.Users.UserInfoList.Where(user => !user.Name.Contains("admin")).Select(
            user => $"user name: {user.Name}; user password {user.Password}; user restrictions: {Shared.ParseRestrictions(user.Restrictions)}");

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}