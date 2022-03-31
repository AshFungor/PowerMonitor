using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace PowerMonitor.controllers;

public class NetworkController
{
    // example
    private readonly HttpClient _httpClient = new();
    public string ServerUri { get; set; } = string.Empty;
    public List<int> Complexes { get; } = new() {123, 111, 345};

    public async void CreateUserAsync(LoginController.UserInfo info)
    {
        var content = new
        {
            admin = new
            {
                login = LoginController.CurrentUser,
                password = LoginController.CurrentPassword
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

        var response = await _httpClient.PostAsync(ServerUri, data);
    }
}