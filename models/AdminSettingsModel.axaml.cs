using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PowerMonitor.services;

namespace PowerMonitor.models;

public class AdminSettingsTab : UserControl
{
    private readonly List<RecordGrid> _records;

    private readonly ListBox _userList;
    private RecordGrid? _editedItem;

    public AdminSettingsTab()
    {
        InitializeComponent();
        var usersList = this.Find<ListBox>("EntitiesListBox");
        var index = 0;
        _userList = usersList;
        _records = new List<RecordGrid>();

        foreach (var user in LoginService.Users.UserInfoList) _records.Add(new RecordGrid(user, index++));

        _userList.Items = _records;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void OnClose(object? sender, LogicalTreeAttachmentEventArgs args)
    {
        if (_editedItem is not null)
            ExitEdit();
    }


    private void TriggerEdit(object? sender, RoutedEventArgs args)
    {
        if (_editedItem is not null)
        {
            if (_userList.SelectedItem is not null)
            {
                if (_editedItem.Equals((RecordGrid) _userList.SelectedItem))
                {
                    ChangeStateRec(_editedItem);
                    ExitEdit();
                }
                else
                {
                    ChangeStateRec(_editedItem);
                    ExitEdit();
                    ChangeStateRec((_userList.SelectedItem as Grid)!);
                    _editedItem = (RecordGrid) _userList.SelectedItem;
                }
            }
        }
        else if (_userList.SelectedItem is not null)
        {
            ChangeStateRec((_userList.SelectedItem as Grid)!);
            _editedItem = (RecordGrid) _userList.SelectedItem;
        }
    }

    private void AddUser(object? sender, RoutedEventArgs args)
    {
        var newUser = new RecordGrid(new LoginService.UserInfo(), _records.Count);

        LoginService.AddUser(new LoginService.UserInfo());

        _records.Add(newUser);
        _userList.Items = _records.ToArray();
    }

    private void DeleteUser(object? sender, RoutedEventArgs args)
    {
        if (_userList.SelectedItem is not null)
        {
            _records[_userList.SelectedIndex].IsVisible = false;
            LoginService.DeleteUser(_userList.SelectedIndex);
        }
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

    private void ExitEdit()
    {
        var target = _editedItem!;

        if (target.IsVisible)
            LoginService.UpdateUser(new LoginService.UserInfo
            {
                Name = target.UserName, Password = target.UserPassword, IsAdmin = target.UserIsAdmin,
                Restrictions = target.Restrictions.Where(rest => rest.Item2).Select(rest => rest.Item1).ToList()
            }, target.OriginalIndex);

        _editedItem = null;
    }

    public class RecordGrid : Grid
    {
        public RecordGrid(LoginService.UserInfo user, int originalIndex)
        {
            UserName = user.Name!;
            UserPassword = user.Password!;
            UserIsAdmin = user.IsAdmin;
            OriginalIndex = originalIndex;


            ColumnDefinitions = new ColumnDefinitions();

            ColumnDefinitions.AddRange(Enumerable.Range(0, 5).Select(el => new ColumnDefinition()));

            TextBox name = new(), password = new();
            var isAdmin = new CheckBox();
            var expander = new Expander();


            // common  
            name.FontSize = password.FontSize = isAdmin.FontSize = expander.FontSize = 24;
            password.HorizontalAlignment =
                isAdmin.HorizontalAlignment = expander.HorizontalAlignment = HorizontalAlignment.Center;
            name.BorderThickness = password.BorderThickness =
                expander.BorderThickness = Thickness.Parse("0");
            name.IsEnabled = password.IsEnabled = isAdmin.IsEnabled = false;
            name.Background = password.Background = isAdmin.Background = expander.Background = Brushes.Transparent;

            // events
            name.LostFocus += NameInputChanged;
            password.LostFocus += PasswordInputChanged;
            isAdmin.Checked += StatusChanged;
            isAdmin.Unchecked += StatusChanged;

            name.Text = user.Name;
            password.Text = user.Password;
            isAdmin.IsChecked = user.IsAdmin;

            Grid rests;
            GenerateRestsGrid(user.Restrictions!, out rests);

            Children.Add(name);
            Children.Add(password);
            Children.Add(isAdmin);

            expander.Header = "Restrictions";
            expander.Content = rests;

            Children.Add(expander);

            SetColumn(name, 0);
            SetColumn(password, 1);
            SetColumn(isAdmin, 2);
            SetColumn(expander, 4);

            PointerEnter += MouseOver;
            PointerLeave += MouseLeft;
        }

        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public bool UserIsAdmin { get; set; }
        public List<(string, bool)> Restrictions { get; set; } = new();
        public int OriginalIndex { get; set; }

        private void GenerateRestsGrid(List<string> rests, out Grid result)
        {
            result = new Grid
            {
                RowDefinitions = new RowDefinitions()
            };
            var row = 0;

            foreach (var complex in LoginService.Complexes)
            {
                result.RowDefinitions.Add(new RowDefinition());

                var status = rests.Contains(complex.ToString());
                var record = new ComplexRecordRest(row, complex.ToString(), status, this);

                Restrictions.Add((complex.ToString(), status));

                result.Children.Add(record);
                SetRow(record, row);
                ++row;
            }
        }

        private void MouseOver(object? sender, EventArgs args)
        {
            Background = new SolidColorBrush(Color.Parse("#1c226c"));
        }

        private void MouseLeft(object? sender, EventArgs args)
        {
            Background = Brushes.Transparent;
        }

        private void NameInputChanged(object? sender, RoutedEventArgs args)
        {
            UserName = ((TextBox) sender!).Text;
        }

        private void PasswordInputChanged(object? sender, RoutedEventArgs args)
        {
            UserPassword = ((TextBox) sender!).Text;
        }

        private void StatusChanged(object? sender, RoutedEventArgs args)
        {
            UserIsAdmin = (bool) ((CheckBox) sender!).IsChecked!;
        }

        public class ComplexRecordRest : Grid
        {
            private readonly string _comlexNum;
            private readonly int _index;
            private readonly RecordGrid _parent;

            public ComplexRecordRest(int index, string complex, bool accessIsAllowed, RecordGrid parent)
            {
                ColumnDefinitions = new ColumnDefinitions();
                ColumnDefinitions.Add(new ColumnDefinition());
                ColumnDefinitions.Add(new ColumnDefinition());

                var complexNumber = new Label();
                var isAllowed = new CheckBox();

                complexNumber.FontSize = isAllowed.FontSize = 24;
                complexNumber.Content = complex;
                isAllowed.IsChecked = accessIsAllowed;
                isAllowed.IsEnabled = false;

                isAllowed.Unchecked += OnStatusChanged;
                isAllowed.Checked += OnStatusChanged;

                Children.Add(complexNumber);
                Children.Add(isAllowed);

                SetColumn(complexNumber, 0);
                SetColumn(isAllowed, 1);

                _comlexNum = complex;
                _parent = parent;
                _index = index;
            }

            public void OnStatusChanged(object? sender, RoutedEventArgs args)
            {
                _parent.Restrictions[_index] = (_comlexNum, (bool) (sender as CheckBox)!.IsChecked!);
            }
        }
    }
}