using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using SimpleLogger;
using Independentsoft.Office.Odf;

namespace PowerMonitor.controllers;

public sealed class DataController
{
    // this will change a lot in the future
    private static readonly string ResponseDataFileLocation = App.SettingsPath + "response.csv";
    private static readonly string SpreadsheetFileBase = App.SettingsPath + "data";
    private CsvConfiguration _csvConfig;

    public class DevInfo
    {
        [Index(0)] public string? Begin { get; set; }
        [Index(1)] public string? End { get; set; }
        [Index(2)] public double ReactivePowerA { get; set; }
        [Index(3)] public double ReactivePowerB { get; set; }
        [Index(4)] public double ReactivePowerC { get; set; }
        [Index(5)] public double ActivePowerA { get; set; }
        [Index(6)] public double ActivePowerB { get; set; }
        [Index(7)] public double ActivePowerC { get; set; }
        [Index(8)] public double VoltageA { get; set; }
        [Index(9)] public double VoltageB { get; set; }
        [Index(10)] public double VoltageC { get; set; }
        [Index(11)] public double CosA { get; set; }
        [Index(12)] public double CosB { get; set; }
        [Index(13)] public double CosC { get; set; }
        [Index(14)] public double UReactivePowerA { get; set; }
        [Index(15)] public double UReactivePowerB { get; set; }
        [Index(16)] public double UReactivePowerC { get; set; }
        [Index(17)] public double UActivePowerA { get; set; }
        [Index(18)] public double UActivePowerB { get; set; }
        [Index(19)] public double UActivePowerC { get; set; }
        [Index(20)] public double UVoltageA { get; set; }
        [Index(21)] public double UVoltageB { get; set; }
        [Index(22)] public double UVoltageC { get; set; }
        [Index(23)] public double UCosA { get; set; }
        [Index(24)] public double UCosB { get; set; }
        [Index(25)] public double UCosC { get; set; }
        [Index(26)] public int N { get; set; }
        
        
    }

    public DataController()
    {
        _csvConfig = new CsvConfiguration(CultureInfo.InstalledUICulture)
        {
            HasHeaderRecord = false, NewLine = Environment.NewLine
        };
    }

    private Task<IEnumerable<DevInfo>> ReadResponseAsync()
    {
        var stream = new StreamReader(ResponseDataFileLocation);
        var csvReader = new CsvReader(stream, _csvConfig);
        var records = csvReader.GetRecords<DevInfo>();
        return Task.FromResult(records);
    }

    public async Task<List<(double, double)>> EvaluateDataAsync(DateTime day)
    {
        var records = await ReadResponseAsync();
        List<(double, double)> results = new List<(double, double)>();
        
        
        var enumerator = records.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var record = enumerator.Current;
            {
                DateTime start = Convert.ToDateTime(record.Begin), finish = Convert.ToDateTime(record.End);
                day =
                    day.AddSeconds(start.Second - day.Second).AddMinutes(start.Minute - day.Minute).AddHours(start.Hour - day.Hour);
                
                
                double sumUPower = record.UActivePowerA + record.UActivePowerB + record.UActivePowerC;
                if (sumUPower == 0)
                {
                    results.Add((100, (day.Hour * 60 + day.Minute) / 60));
                    if (start != finish)
                    {
                        day =
                            day.AddSeconds(finish.Second - day.Second).AddMinutes(finish.Minute - day.Minute).AddHours(finish.Hour - day.Hour);
                        results.Add((100, (day.Hour * 60 + day.Minute) / 60));
                    }
                    continue;
                }
                double effectiveness =
                    (sumUPower - (record.ActivePowerA + record.ActivePowerB + record.ActivePowerC)) / (sumUPower) * 100;
                results.Add((effectiveness, (day.Hour * 60 + day.Minute) / 60));
                if (start != finish)
                {
                    day =
                        day.AddSeconds(finish.Second - day.Second).AddMinutes(finish.Minute - day.Minute)
                            .AddHours(finish.Hour - day.Hour);
                    results.Add((effectiveness, (day.Hour * 60 + day.Minute) / 60));
                }
            }
        }
        enumerator.Dispose();

        return results;

    }

    public Task<bool> LoadIntoSpreadsheet(ref IEnumerable<DevInfo> data)
    {
        Table table = new Table();
        int column = 1;
        
        
        foreach (var field in typeof(DevInfo).GetFields())
        {
            table[column, 1] = new Cell(field.Name);
            ++column;
        }

        Spreadsheet sheet = new Spreadsheet();
        sheet.Tables.Add(table);

        string acceptableName = SpreadsheetFileBase;
        int fileIndex = 1;
        while (File.Exists(acceptableName + fileIndex + ".ods"))
            ++fileIndex;
        
        sheet.Save(acceptableName + fileIndex + ".ods");

        return Task.FromResult(true);

    }
}