using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SimpleLogger;

namespace PowerMonitor.models;

public class Login : UserControl
{
    private TextBox loginInput;
    private TextBox passwordInput;
    private Button loginButton;
    private Label logLabel;
    private string password = String.Empty;
    private string login = String.Empty;
    public Login()
    {
        Logger.Log<Login>("building login view");
        InitializeComponent();
        loginInput = this.FindControl<TextBox>("LoginInput");
        passwordInput = this.FindControl<TextBox>("PasswordInput");
        loginButton = this.FindControl<Button>("LoginButton");
        logLabel = this.FindControl<Label>("LogLabel");
        
        loginButton.Click += TryLogin;
        passwordInput.PasswordChar = '*';
        passwordInput.RevealPassword = false;

        Logger.Log<Login>("built login view");
    }

    private void TryLogin(object? sender, EventArgs args)
    {
        password = passwordInput.Text;
        login = loginInput.Text;
        Logger.Log<Login>($"Attempting to log with ps = {password} and login = {login}");
        foreach (var match in Shared.LoginController.Users.UserInfoList)
        {
            Logger.Log<Login>($"checking {match.Name} with pas = {match.Password}");
            if (match.Password != null && match.Name != null && match.Name.Equals(login) && match.Password.Equals(password))
            {
                if (login.Equals("admin"))
                    Shared.MainWin.Content = new AdminView();
                else
                    Shared.MainWin.Content = new UserView();
                Logger.Log<Login>("Attempt successful");
                return;
            }
        }
        
        Logger.Log<Login>(Logger.Level.Error,"Attempt unsuccessful");
        logLabel.Content = "auf is failed";

    }
    

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}