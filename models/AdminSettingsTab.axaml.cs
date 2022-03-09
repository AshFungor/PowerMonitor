using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PowerMonitor.models;

public partial class AdminSettingsTab : UserControl
{
    private ListBox _entities;
    public AdminSettingsTab()
    {
        InitializeComponent();

        _entities = this.Find<ListBox>("EntitiesListBox");
        List<DockPanel> items = new List<DockPanel>();
        foreach (var entity in Shared.LoginController!.Users!.UserInfoList!)
        {
            if (entity.Restrictions is null) entity.Restrictions = new List<string>();
            DockPanel item = new DockPanel();
            item.Children.Add(new TextBox()
                { IsReadOnly = true, Height = 50, Text = entity.Name, FontSize = 24, 
                    Margin = Thickness.Parse("0 0 10 0") });
            item.Children.Add(new TextBox()
                { IsReadOnly = true, Height = 50, Text = entity.Password, FontSize = 24, 
                    Margin = Thickness.Parse("10 0 10 0") });
            List<Control> contents = new List<Control>() { new Label() {Content = "click here to look", FontSize = 24 }};
            foreach (var dev in Shared.NetworkController!.devs)
            {
                DockPanel dockItem = new DockPanel();
                dockItem.Children.Add(new CheckBox() { IsChecked = entity.Restrictions.Contains(dev.ToString()), FontSize = 24 });
                dockItem.Children.Add(new Label() { Content = dev, FontSize = 24 });
                contents.Add(dockItem);
            }

            ComboBox temp = new ComboBox() { Items = contents, FontSize = 24, SelectedIndex = 0 };
            temp.SelectionChanged += (sender, args) => ((ComboBox) sender!).SelectedIndex = 0; 
            item.Children.Add(temp);
            items.Add(item);
        }

        _entities.Items = items;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}