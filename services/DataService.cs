using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using ExtremelySimpleLogger;
using GemBox.Spreadsheet;
using SPath = PowerMonitor.services.SettingsService.Settings;

namespace PowerMonitor.services;

public static class DataService
{
    // paths to spreadsheet and response from server files 
    private static readonly string ResponseDataFileLocation = SPath.TempFolder + "response.csv";
    private static readonly string SpreadsheetFileBase = SPath.DataFolder + "data";

    private static readonly List<PropertyInfo> SavingProperties =
        typeof(DevInfo).GetProperties().Where(el => !(el.Name.Contains("N") || el.Name.Contains("Voltage"))).ToList();

    private static readonly CsvConfiguration CsvConfig;

    static DataService()
    {
        CsvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = false, NewLine = Environment.NewLine, Delimiter = ";"
        };
    }

    private static Task<IEnumerable<DevInfo>> ReadResponseAsync()
    {
        Shared.Logger!.Log(LogLevel.Info, "async reading of response");

        // read response from server, mostly runs async
        var stream = new StreamReader(ResponseDataFileLocation);
        var csvReader = new CsvReader(stream, CsvConfig);
        var records = csvReader.GetRecords<DevInfo>();
        return Task.FromResult(records);
    }

    private static void FormatResponse()
    {
        StreamReader istream = new StreamReader(ResponseDataFileLocation);
        List<string[]> lines = new List<string[]>();
        while (!istream.EndOfStream)
        {
            var line = istream.ReadLine().Split(';');
            for (int i = 0; i < line.Length; ++i)
            {
                line[i] = (line[i].Equals(string.Empty)) ? "0" : line[i];
            }
            lines.Add(line);
        }
        istream.Close();
        File.WriteAllText(ResponseDataFileLocation, "");
        string text = string.Empty;
        foreach (var line in lines)
        {
            text += string.Join(';', line) + "\n";
        }
        File.WriteAllText(ResponseDataFileLocation, text);
    }

    public static async Task<List<(double, double)>> EvaluateDataAsync(DateTime day)
    {
        FormatResponse();
        var records = await ReadResponseAsync();
        var results = new List<(double, double)>();

        // parsing response
        Shared.Logger!.Log(LogLevel.Info, "beginning parsing response.csv...");
        var enumerator = records.GetEnumerator();
        int hour = day.Hour;
        
        while (enumerator.MoveNext())
        {
            var record = enumerator.Current;
            {
                DateTime start = DateTime.ParseExact(record.Begin, "dd.MM.yyyy  HH:mm:ss", CultureInfo.InvariantCulture),
                    finish = DateTime.ParseExact(record.End, "dd.MM.yyyy  HH:mm:ss", CultureInfo.InvariantCulture);
                hour += (finish.Hour - start.Hour) + (finish.Second - start.Second);
                    


                var sumUPower = record.UActivePowerA + record.UActivePowerB + record.UActivePowerC;
                if (sumUPower == 0)
                {
                    results.Add((100, hour));
                    if (start != finish)
                    {
                        hour += (finish.Hour - start.Hour) + (finish.Second - start.Second);
                        results.Add((100, hour));
                    }

                    continue;
                }

                var effectiveness =
                    (sumUPower - (record.ActivePowerA + record.ActivePowerB + record.ActivePowerC)) / sumUPower * 100;
                results.Add((effectiveness, hour));
                if (start != finish)
                {
                    hour += (finish.Hour - start.Hour) + (finish.Second - start.Second);
                    results.Add((effectiveness, hour));
                }
            }
        }

        enumerator.Dispose();

        Shared.Logger.Log(LogLevel.Info, "finished parse.");
        return results;
    }

    public static async Task<bool> LoadIntoSpreadsheetAsync()
    {
        // save to *.odt file current data, actually it is just last response from server
        var data = await ReadResponseAsync();
        Shared.Logger.Log(LogLevel.Info, "parsing response complete, beginning writing...");

        var workBook = new ExcelFile();
        var sheet = workBook.Worksheets.Add("parsed data");

        var column = 0;


        foreach (var property in SavingProperties)
        {
            sheet.Cells[column, 0].Value = property.Name;
            ++column;
        }

        sheet.Cells[column, 0].Value = "Effectiveness";
        var row = 1;
        var enumerator = data.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var record = enumerator.Current;
            column = 0;
            foreach (var property in SavingProperties)
            {
                var fieldData = property.GetValue(record);
                if (property.Name.Equals("Begin") || property.Name.Equals("End"))
                    sheet.Cells[column, row].Value = (string) (fieldData ?? "NO_DATA");
                else
                    sheet.Cells[column, row].Value = (double) (fieldData ?? 0);
                ++column;
            }

            var sumUPower = record.UActivePowerA + record.UActivePowerB + record.UActivePowerC;
            if (sumUPower == 0)
                sheet.Cells[column, row].Value = 100;
            else
                sheet.Cells[column, row].Value =
                    (sumUPower - (record.ActivePowerA + record.ActivePowerB + record.ActivePowerC)) / sumUPower * 100;
            ++row;
        }

        enumerator.Dispose();
        Shared.Logger.Log(LogLevel.Info, "writing complete, saving...");

        var acceptableName = SpreadsheetFileBase;
        var fileIndex = 1;
        while (File.Exists(acceptableName + fileIndex + ".ods"))
            ++fileIndex;

        workBook.Save(acceptableName + fileIndex + ".ods");

        Shared.Logger.Log(LogLevel.Info, "save complete.");
        return true;
    }

    public static bool CheckResponse()
    {
        Shared.Logger!.Log(LogLevel.Info, "checking response...");

        // check if server ever sent something and there is a file to work with
        return File.Exists(ResponseDataFileLocation);
    }

    // hell called response from server file line
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
}