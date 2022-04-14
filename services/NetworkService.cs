using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using ExtremelySimpleLogger;
using Newtonsoft.Json;

namespace PowerMonitor.services;

public static class NetworkService
{
    private static readonly HttpClient HttpClient = new();
    public static string ServerUri { get; } = SettingsService.Settings.ServerAddress;
    public static List<int> Complexes { get; } = new() {123, 111, 345};

    public static async void CreateUserAsync(LoginService.UserInfo info)
    {
        var content = new
        {
            admin = new
            {
                login = LoginService.CurrentUser,
                password = LoginService.CurrentPassword
            },
            user = new
            {
                login = info.Name,
                password = info.Password,
                is_admin = info.IsAdmin,
                Complexes = info.Restrictions!.ToArray()
            }
        };

        var jsonString = JsonConvert.SerializeObject(content);
        Shared.Logger!.Log(LogLevel.Info, $"preparing to send a request with body: {jsonString}");

        var data = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var responseMessage = await HttpClient.PostAsync(ServerUri + "/create-user", data);
        Shared.Logger!.Log(LogLevel.Info, $"response: {responseMessage.Content}");
        
    }

    public static async void DeleteUserAsync(LoginService.UserInfo info)
    {
        var content = new
        {
            admin = new
            {
                login = LoginService.CurrentUser,
                password = LoginService.CurrentPassword
            },
            user = new
            {
                login = info.Name,
            }
        };

        var jsonString = JsonConvert.SerializeObject(content);
        Shared.Logger!.Log(LogLevel.Info, $"preparing to send a request with body: {jsonString}");

        var data = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var responseMessage = await HttpClient.PostAsync(ServerUri + "/delete-user", data);
        Shared.Logger!.Log(LogLevel.Info, $"response: {responseMessage.Content}");
    }

    public static void UpdateUserAsync(LoginService.UserInfo prevUser, LoginService.UserInfo newUser)
    {
        DeleteUserAsync(prevUser);
        CreateUserAsync(newUser);
    }
    
    
}