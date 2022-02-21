using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SimpleLogger;

namespace PowerMonitor.models;

public class Login : UserControl
{
    private readonly TextBox _loginInput;
    private readonly TextBox _passwordInput;
    private readonly Label _logLabel;
    private string _password = string.Empty;
    private string _login = string.Empty;

    public Login()
    {
        Logger.Log<Login>("building login view");
        InitializeComponent();
        _loginInput = this.FindControl<TextBox>("LoginInput");
        _passwordInput = this.FindControl<TextBox>("PasswordInput");
        _logLabel = this.FindControl<Label>("LogLabel");

#if DEBUG && !SERVER
        _passwordInput.Text = "password";
        _loginInput.Text = "admin";
#endif

        Logger.Log<Login>("built login view");
    }

    private void TryLogin(object? sender, RoutedEventArgs args)
    {
#if !SERVER
        _password = _passwordInput.Text;
        _login = _loginInput.Text;
        Logger.Log<Login>($"Attempting to log with ps = {_password} and login = {_login}");
        foreach (var match in Shared.LoginController!.Users!.UserInfoList!)
        {
            Logger.Log<Login>($"checking {match.Name} with pas = {match.Password}");
            if (match.Password != null && match.Name != null && match.Name.Equals(_login) &&
                match.Password.Equals(_password))
            {
                Shared.MainWin!.Content = _login.Equals("admin") ? new AdminView() : new UserView();
                Logger.Log<Login>("Attempt successful");
                return;
            }
        }

        Logger.Log<Login>(Logger.Level.Error, "Attempt unsuccessful");
        _logLabel.Content = "try again";
#endif
    }


    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}