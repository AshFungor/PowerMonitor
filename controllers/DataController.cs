using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using CsvHelper;
using CsvHelper.Configuration;
using SimpleLogger;

namespace PowerMonitor.controllers;

public sealed class DataController
{
    // this will change a lot in the future
    private string _serverRequest = string.Empty;
    private static string _dataFileLocation = App.SettingsPath + "response.csv";
    private static string _feedTemplateReq = "localhost:5000/{0}/{1}";
    private HttpClient _localClient;

    public DataController()
    {
        _localClient = new HttpClient();
    }

    public async void GetData(DateTime date)
    {
        var msg =
            new HttpRequestMessage(HttpMethod.Get, new Uri(string.Format(_feedTemplateReq, date)));

        Logger.Log<DataController>(Logger.Level.Info, $"sending ({msg}), awaiting...");
        var response = await _localClient.SendAsync(msg);
        Logger.Log<DataController>(Logger.Level.Info,
            $"received: {response.StatusCode}, {response.Headers}, waiting for reading...");
        try
        {
            var inputStream = new StreamReader(await response.Content.ReadAsStreamAsync());
            var outputStream = new StreamWriter(_dataFileLocation);
            while (!inputStream.EndOfStream)
                await outputStream.WriteLineAsync(await inputStream.ReadLineAsync());
        }
        catch (Exception e)
        {
            // ignored
            Logger.Log<DataController>(Logger.Level.Error, e.Message);
        }
    }
}