using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace PowerMonitor.models;

public class AdminSettingsTab : UserControl
{
    private readonly ListBox _entities;
    private Grid? _editedItem = null;

    public AdminSettingsTab()
    {
        InitializeComponent();

        var entities = this.Find<ListBox>("EntitiesListBox");
        _entities = entities;
        entities.SelectionMode = SelectionMode.Single;
        var items = new List<Grid>();
        foreach (var entity in Shared.LoginController!.Users!.UserInfoList!)
        {
            if (entity.Restrictions is null) entity.Restrictions = new List<string>();
            var item = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions(),
                RowDefinitions = new RowDefinitions()
            };

            var name = new TextBox
            {
                IsReadOnly = true, Height = 50, Text = entity.Name, FontSize = 24,
                Margin = Thickness.Parse("0 0 10 0"), Background = Brushes.Transparent,
                BorderThickness = Thickness.Parse("0"), IsEnabled = false
            };
            var password = new TextBox
            {
                IsReadOnly = true, Height = 50, Text = entity.Password, FontSize = 24,
                Margin = Thickness.Parse("10 0 10 0"), Background = Brushes.Transparent,
                BorderThickness = Thickness.Parse("0"),
                HorizontalAlignment = HorizontalAlignment.Center, IsEnabled = false
            };
            var isAdmin = new CheckBox
            {
                FontSize = 30, IsEnabled = false, IsChecked = entity.IsAdmin,
                Margin = Thickness.Parse("10 0 10 0"), Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            item.ColumnDefinitions.Add(new ColumnDefinition());
            item.ColumnDefinitions.Add(new ColumnDefinition());
            item.ColumnDefinitions.Add(new ColumnDefinition());
            item.ColumnDefinitions.Add(new ColumnDefinition());
            item.ColumnDefinitions.Add(new ColumnDefinition());


            item.Children.Add(name);
            item.Children.Add(password);
            item.Children.Add(isAdmin);
            Grid.SetColumn(name, 0);
            Grid.SetColumn(password, 1);
            Grid.SetColumn(isAdmin, 2);

            item.PointerEnter += MouseOver;
            item.PointerLeave += MouseLeft;

            var contents = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions(),
                RowDefinitions = new RowDefinitions()
            };
            contents.ColumnDefinitions.Add(new ColumnDefinition());
            contents.ColumnDefinitions.Add(new ColumnDefinition());
            var row = 0;

            foreach (var dev in Shared.NetworkController!.Complexes)
            {
                var devNumber = new Label {Content = dev, FontSize = 24};
                var isAllowed = new CheckBox
                    {IsChecked = entity.Restrictions.Contains(dev.ToString()), FontSize = 24, IsEnabled = false};

                contents.RowDefinitions.Add(new RowDefinition());

                contents.Children.Add(devNumber);
                contents.Children.Add(isAllowed);
                Grid.SetColumn(isAllowed, 0);
                Grid.SetColumn(devNumber, 1);
                Grid.SetRow(isAllowed, row);
                Grid.SetRow(devNumber, row);
                ++row;
            }

            var expander = new Expander {FontSize = 24, BorderThickness = Thickness.Parse("0")};
            expander.Header = "Restrictions";
            expander.HorizontalAlignment = HorizontalAlignment.Center;
            expander.Content = contents;
            item.Children.Add(expander);
            Grid.SetColumn(expander, 4);
            items.Add(item);
        }

        entities.Items = items;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MouseOver(object? sender, EventArgs args)
    {
        var element = sender as Grid;
        var activeElement = _entities.SelectedItem as Grid;
        if (element!.Name != (activeElement?.Name ?? string.Empty))
            element!.Background = new SolidColorBrush(Color.Parse("#1c226c"));
    }

    private void MouseLeft(object? sender, EventArgs args)
    {
        var element = sender as Grid;
        element!.Background = Brushes.Transparent;
    }

    private void EnterEdit(object? sender, RoutedEventArgs args)
    {
        if (_entities.SelectedItem is not null) ChangeStateRec((_entities.SelectedItem as Grid)!);
    }

    private void ChangeStateRec(Control control)
    {
        if (control is Expander expander)
            foreach (var child in (expander.Content as Grid)!.Children)
                ChangeStateRec((child as Control)!);
        else if (control is Panel panel)
            foreach (var child in panel.Children)
                ChangeStateRec((child as Control)!);
        else if (control is not Expander or Label) control.IsEnabled = !control.IsEnabled;
    }
}