using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SimpleLogger;
using Key = Avalonia.Remote.Protocol.Input.Key;

namespace PowerMonitor.views;

public class Login : UserControl
{
    private TextBox loginInput;
    private TextBox passwordInput;
    private string password = String.Empty;
    public Login()
    {
        InitializeComponent();
        loginInput = this.FindControl<TextBox>("LoginInput");
        passwordInput = this.FindControl<TextBox>("PasswordInput");
        

        passwordInput.KeyDown += BlurPassword;
        passwordInput.PasswordChar = '*';
        passwordInput.RevealPassword = false;


    }

    private void BlurPassword(object? sender, KeyEventArgs args)
    {
        if (args.Key.ToString().Length == 1 && (Char.IsLetter(Convert.ToChar(args.Key.ToString())) ||
                                                Char.IsNumber(Convert.ToChar(args.Key.ToString()))))
        {
            password += args.Key.ToString();
            Logger.Log<Login>($"password updated: {password}");
        }
    }
    

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}