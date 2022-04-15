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

        var res = LoginService.LogIn(_login, _password);

        if (res)
        {
            Shared.Logger.Log(LogLevel.Info, "Login successful");
            Shared.MainWin!.Content = LoginService.AdminStatus ? new AdminView() : new UserView();
            return;
        }

        _logLabel.Content = "Try again";
    }


    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}