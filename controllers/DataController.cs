using System;
using System.IO;
using System.Net;
using CsvHelper;
using SimpleLogger;

namespace PowerMonitor.controllers;

public sealed class DataController
{
    private string _serverRequest = string.Empty;
    private string _dataFileLocation = App.SettingsPath + "response.csv";

    public DataController()
    {
    }

    public async void GetData(DateTime date)
    {
    }
}