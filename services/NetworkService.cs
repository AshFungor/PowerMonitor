using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ExtremelySimpleLogger;
using Newtonsoft.Json;

namespace PowerMonitor.services;

public static class NetworkService
{
    private static readonly HttpClient HttpClient = new();
    public static string ServerUri { get; } = SettingsService.Settings.ServerAddress;
    public static List<int> Complexes { get; set; } = new() {123, 111, 345};

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
        Shared.Logger!.Log(LogLevel.Info, $"response: {responseMessage.Content.Headers}");
        
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
        Shared.Logger!.Log(LogLevel.Info, $"response: {responseMessage.Content.Headers}");
    }

    public static void UpdateUserAsync(LoginService.UserInfo prevUser, LoginService.UserInfo newUser)
    {
        DeleteUserAsync(prevUser);
        CreateUserAsync(newUser);
    }

    public static async Task<LoginInfo?> LogIn(string login, string password)
    {
        var content = new
        {
            user = new 
            {
                login = login,
                password = password
            }
        };

        var jsonString = JsonConvert.SerializeObject(content);
        Shared.Logger!.Log(LogLevel.Info, $"preparing to send a request with body: {jsonString}");

        var msg = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            Content = new StringContent(jsonString, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(ServerUri + "/get-user-info")
        };
        
        var response = await HttpClient.SendAsync(msg).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Shared.Logger.Log(LogLevel.Error, "login unsuccessful");
            return null;
        }

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var aResponse = new
        {
            complexes = Array.Empty<int>(),
            is_admin = false
        };

        aResponse = JsonConvert.DeserializeAnonymousType(responseBody, aResponse);
        var info = new LoginInfo(aResponse.is_admin, aResponse.complexes);
        
        Shared.Logger.Log(LogLevel.Info, info.ToString());

        return info;

    }

    public class LoginInfo
    {
        public LoginInfo(bool isAdmin, int[] complexes)
        {
            IsAdmin = isAdmin;
            Complexes = complexes;
        }

        public override string ToString()
        {
            return "complexes: " + string.Join(',', Complexes) +
                   "\nis_admin: " + IsAdmin;
        }

        public bool IsAdmin { get; set; }
        public int[] Complexes { get; set; }
    }
    
    
}