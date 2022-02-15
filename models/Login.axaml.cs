using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SimpleLogger;

namespace PowerMonitor.models;

public class Login : UserControl
{
    private TextBox loginInput;
    private TextBox passwordInput;
    private Label logLabel;
    private string _password = string.Empty;
    private string _login = string.Empty;

    public Login()
    {
        Logger.Log<Login>("building login view");
        InitializeComponent();
        loginInput = this.FindControl<TextBox>("LoginInput");
        passwordInput = this.FindControl<TextBox>("PasswordInput");
        var loginButton = this.FindControl<Button>("LoginButton");
        logLabel = this.FindControl<Label>("LogLabel");

        loginButton.Click += TryLogin;
        passwordInput.PasswordChar = '*';
        passwordInput.RevealPassword = false;

#if DEBUG
        passwordInput.Text = "password";
        loginInput.Text = "admin";
#endif

        Logger.Log<Login>("built login view");
    }

    private void TryLogin(object? sender, EventArgs args)
    {
        _password = passwordInput.Text;
        _login = loginInput.Text;
        Logger.Log<Login>($"Attempting to log with ps = {_password} and login = {_login}");
        foreach (var match in Shared.LoginController.Users.UserInfoList)
        {
            Logger.Log<Login>($"checking {match.Name} with pas = {match.Password}");
            if (match.Password != null && match.Name != null && match.Name.Equals(_login) &&
                match.Password.Equals(_password))
            {
                if (_login.Equals("admin"))
                    Shared.MainWin.Content = new AdminView();
                else
                    Shared.MainWin.Content = new UserView();
                Logger.Log<Login>("Attempt successful");
                return;
            }
        }

        Logger.Log<Login>(Logger.Level.Error, "Attempt unsuccessful");
        logLabel.Content = "try again";
    }


    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}