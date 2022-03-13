using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExtremelySimpleLogger;

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
#if !SERVER
        _password = _passwordInput.Text;
        _login = _loginInput.Text;
        Shared.Logger!.Log(LogLevel.Info, $"Attempting to log with ps = {_password} and login = {_login}");
        foreach (var match in Shared.LoginController!.Users!.UserInfoList!)
        {
            Shared.Logger!.Log(LogLevel.Info, $"checking {match.Name} with pas = {match.Password}");
            if (match.Password != null && match.Name != null && match.Name.Equals(_login) &&
                match.Password.Equals(_password))
            {
                Shared.MainWin!.Content = _login.Equals("admin") ? new AdminView() : new UserView();
                Shared.Logger!.Log(LogLevel.Warn, "Attempt successful");
                return;
            }
        }

        Shared.Logger!.Log(LogLevel.Error, "Attempt unsuccessful");
        _logLabel.Content = "try again";
#endif
    }


    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}