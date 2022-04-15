using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                Complexes = info.Restrictions!.Select(int.Parse).ToArray()
            }
        };

        var jsonString = JsonConvert.SerializeObject(content);
        Shared.Logger!.Log(LogLevel.Info, $"preparing to send a request with body: {jsonString}");

        var data = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var responseMessage = await HttpClient.PostAsync(ServerUri + "/create-user", data);
        Shared.Logger!.Log(LogLevel.Info, $"response: {responseMessage.Content.ReadAsStringAsync()}");
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
            user_login = info.Name
        };

        var jsonString = JsonConvert.SerializeObject(content);
        Shared.Logger!.Log(LogLevel.Info, $"preparing to send a request with body: {jsonString}");

        var data = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var responseMessage = await HttpClient.PostAsync(ServerUri + "/delete-user", data);
        Shared.Logger!.Log(LogLevel.Info, $"response: {responseMessage.Content.ReadAsStringAsync()}");
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
                login, password
            }
        };

        var jsonString = JsonConvert.SerializeObject(content);
        Shared.Logger!.Log(LogLevel.Info, $"preparing to send a request with body: {jsonString}");

        var msg = new HttpRequestMessage
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
        Shared.Logger.Log(LogLevel.Info, $"received message: {responseBody}");

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

    public static async Task<bool> GetUsers()
    {
        var content = new
        {
            admin = new
            {
                login = LoginService.CurrentUser,
                password = LoginService.CurrentPassword
            }
        };

        var jsonString = JsonConvert.SerializeObject(content);
        Shared.Logger!.Log(LogLevel.Info, $"preparing to send a request with body: {jsonString}");

        var msg = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            Content = new StringContent(jsonString, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(ServerUri + "/get-all-users")
        };

        var response = await HttpClient.SendAsync(msg).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            return false;
        }

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var users = JsonConvert.DeserializeObject<List<User>>(responseBody);
        LoginService.Users.UserInfoList = users.Select(userData =>
                new LoginService.UserInfo(userData.Name, userData.Password, userData.Complexes, userData.IsAdmin))
            .ToArray();
        return true;
    }

    public static string ParseDateTime(DateTime target)
    {
        return string.Join('.', target.Day, target.Month, target.Year);
    }

    public static async Task<bool> GetData(DateTime start, DateTime end, int id)
    {
        var content = new
        {
            user = new
            {
                login = LoginService.CurrentUser,
                password = LoginService.CurrentPassword
            },
            start = ParseDateTime(start),
            end = ParseDateTime(end),
            complex_id = id
        };

        var jsonString = JsonConvert.SerializeObject(content);
        Shared.Logger!.Log(LogLevel.Info, $"preparing to send a request with body: {jsonString}");

        var msg = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            Content = new StringContent(jsonString, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(ServerUri + "/get-data")
        };

        var response = await HttpClient.SendAsync(msg).ConfigureAwait(false);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            return false;
        }

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        await File.WriteAllTextAsync(SettingsService.Settings.TempFolder + "response.csv", responseBody);

        return true;
    }

    public class LoginInfo
    {
        public LoginInfo(bool isAdmin, int[] complexes)
        {
            IsAdmin = isAdmin;
            Complexes = complexes;
        }

        public bool IsAdmin { get; set; }
        public int[] Complexes { get; set; }

        public override string ToString()
        {
            return "complexes: " + string.Join(',', Complexes) +
                   "\tis_admin: " + IsAdmin;
        }
    }

    public class User
    {
        public User(List<string> complexes, bool isAdmin, string name, string password)
        {
            Complexes = complexes;
            IsAdmin = isAdmin;
            Name = name;
            Password = password;
        }

        [JsonProperty("complex_ids")] public List<string> Complexes { get; set; }

        [JsonProperty("is_admin")] public bool IsAdmin { get; set; }

        [JsonProperty("login")] public string Name { get; set; }

        [JsonProperty("password")] public string Password { get; set; }
    }
}