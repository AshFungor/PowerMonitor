using System;
using System.Net;
using CsvHelper;
using SimpleLogger;

namespace PowerMonitor.controllers;

public sealed class DataController
{
    private string _serverRequest = String.Empty;
    private string _dataFileLocation = App.SettingsPath + "response.csv";

    public DataController()
    {
        
        
    }
    
    
    
}