using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace PowerMonitor.services;

public class NetworkController
{
    // example
    private readonly HttpClient _httpClient = new();
    public string ServerUri { get; set; } = string.Empty;
    public List<int> Complexes { get; } = new() {123, 111, 345};

    public async void CreateUserAsync(LoginService.UserInfo info)
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
        Console.WriteLine(jsonString);

        var data = new StringContent(jsonString, Encoding.UTF8, "application/json");

        await _httpClient.PostAsync(ServerUri, data);
    }
}