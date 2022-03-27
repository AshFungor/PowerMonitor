using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PowerMonitor.controllers;

namespace PowerMonitor.models;

public class AdminSettingsTab : UserControl
{
    
    private RecordGrid? _editedItem = null;
    private ListBox _userList;

    public class RecordGrid : Grid
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public bool UserIsAdmin { get; set; }
        public List<(string, bool)> Restrictions { get; set; } = new List<(string, bool)>();

        public RecordGrid(LoginController.UserInfo user) : base()
        {
            UserName = user.Name!;
            UserPassword = user.Password!;
            UserIsAdmin = user.IsAdmin;


            ColumnDefinitions = new ColumnDefinitions();
            
            ColumnDefinitions.AddRange(Enumerable.Range(0, 5).Select((el) => new ColumnDefinition()));

            TextBox name = new TextBox(), password = new TextBox();
            CheckBox isAdmin = new CheckBox(); Expander expander = new Expander();
            
            
            // common  
            name.FontSize = password.FontSize = isAdmin.FontSize = expander.FontSize = 24;
            password.HorizontalAlignment = isAdmin.HorizontalAlignment = expander.HorizontalAlignment = HorizontalAlignment.Center;
            name.BorderThickness = password.BorderThickness = isAdmin.BorderThickness = expander.BorderThickness = Thickness.Parse("0");
            name.IsEnabled = password.IsEnabled = isAdmin.IsEnabled = false;
            name.Background = password.Background = isAdmin.Background = expander.Background = Brushes.Transparent;
            
            // events
            name.TextInput += (sender, args) => UserName = (sender as TextBlock)!.Text;
            password.TextInput += (sender, args) => UserPassword = (sender as TextBlock)!.Text;
            isAdmin.Checked += (sender, args) => UserIsAdmin = true;
            isAdmin.Unchecked += (sender, args) => UserIsAdmin = false;

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

        private void GenerateRestsGrid(List<string> rests, out Grid result)
        {
            result = new Grid()
            {
                RowDefinitions = new RowDefinitions()
            };
            var row = 0;

            foreach (int complex in Shared.NetworkController!.Complexes)
            {
                result.RowDefinitions.Add(new RowDefinition());
                
                bool status = rests.Contains(complex.ToString());
                ComplexRecordRest record = new ComplexRecordRest(row, complex.ToString(), status, this);
                
                Restrictions.Add((complex.ToString(), status));

                result.Children.Add(record);
                SetRow(record, row);
                ++row;
            }
        }

        public class ComplexRecordRest : Grid
        {
            private int _index;
            private RecordGrid _parent;
            private readonly string _comlexNum;

            public ComplexRecordRest(int index, string complex, bool accessIsAllowed,  RecordGrid parent) : base()
            {
                ColumnDefinitions = new ColumnDefinitions();
                ColumnDefinitions.Add(new ColumnDefinition());
                ColumnDefinitions.Add(new ColumnDefinition());
                
                Label complexNumber = new Label();
                CheckBox isAllowed = new CheckBox();

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
                _parent.Restrictions[_index] = (_comlexNum, (bool)(sender as CheckBox)!.IsChecked!);
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
        
        
    }

    public AdminSettingsTab()
    {
        InitializeComponent();
        ListBox usersList = this.Find<ListBox>("EntitiesListBox");
        _userList = usersList; 
        List<RecordGrid> users = new List<RecordGrid>();
        
        
        foreach (var user in Shared.LoginController.Users.UserInfoList)
        {
            users.Add(new RecordGrid(user));    
        }

        usersList.Items = users;
        
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void EnterEdit(object? sender, RoutedEventArgs args)
    {
        if (_userList.SelectedItem is not null) ChangeStateRec((_userList.SelectedItem as Grid)!);
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
        
        
    }
}