using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExtremelySimpleLogger;
using PowerMonitor.services;

namespace PowerMonitor.models;

public class Login : UserControl
{
    private readonly TextBox _loginInput;
    private readonly Label _logLabel;
    private readonly TextBox _passwordInput;
    private string _login = string.Empty;
    private string _password = string.Empty;

    public Login()
    {
        Shared.Logger!.Log(LogLevel.Info, "building login view");
        InitializeComponent();
        _loginInput = this.FindControl<TextBox>("LoginInput");
        _passwordInput = this.FindControl<TextBox>("PasswordInput");
        _logLabel = this.FindControl<Label>("LogLabel");

#if DEBUG && !SERVER
        _passwordInput.Text = "password";
        _loginInput.Text = "admin";
#endif

        Shared.Logger!.Log(LogLevel.Info, "built login view");
    }

    private void TryLogin(object? sender, RoutedEventArgs args)
    {
        _password = _passwordInput.Text;
        _login = _loginInput.Text;
        Shared.Logger!.Log(LogLevel.Info, $"Attempting to log with ps = {_password} and login = {_login}");

        if (SettingsService.Settings.ServerOn)
        {
            var res = NetworkService.LogIn(_login, _password).Result;
            if (res is null)
            {
                _logLabel.Content = "try again";
            }
            else
            {
                NetworkService.Complexes = res.Complexes.ToList();
                Shared.MainWin!.Content = res.IsAdmin ? new AdminView() : new UserView();
                Shared.Logger!.Log(LogLevel.Warn, "Attempt successful");

                LoginService.CurrentUser = _login;
                LoginService.CurrentPassword = _password;
            }
        }
        else
        {
            foreach (var match in LoginService.Users.UserInfoList!)
            {
                Shared.Logger!.Log(LogLevel.Info, $"checking {match.Name} with pas = {match.Password}");
                if (match.Password != null && match.Name != null && match.Name.Equals(_login) &&
                    match.Password.Equals(_password))
                {
                    Shared.MainWin!.Content = match.IsAdmin ? new AdminView() : new UserView();
                    Shared.Logger!.Log(LogLevel.Warn, "Attempt successful");
                    LoginService.CurrentPassword = match.Password;
                    LoginService.CurrentUser = match.Name;
                    return;
                }
            }
            
            Shared.Logger!.Log(LogLevel.Error, "Attempt unsuccessful");
            _logLabel.Content = "try again";
        }

        
        
    }


    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}