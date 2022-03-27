using System;
using System.IO;
using System.Xml.Serialization;

namespace PowerMonitor.controllers;

public static class SettingsController
{
    private static readonly string SettingsFile = App.SettingsPath + "settings.xml";

    private static bool SettingsPresent()
    {
        return File.Exists(SettingsFile);
    }

    private static void CreatePath(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }


    public static class Settings
    {
        private static readonly SettingsFileXmlTemplate Template;


        static Settings()
        {
            SettingsFileXmlTemplate template;
            if (SettingsPresent())
            {
                using var iStream = new StreamReader(SettingsFile);
                var xmlParser = new XmlSerializer(typeof(SettingsFileXmlTemplate));
                template = (SettingsFileXmlTemplate) xmlParser.Deserialize(iStream)!;

                iStream.Close();
            }
            else
            {
                template = new SettingsFileXmlTemplate();
            }

            DataFolder = template.DataFolder;
            ConfigFolder = template.ConfigFolder;
            TempFolder = template.TempFolder;

            CreatePath(DataFolder);
            CreatePath(ConfigFolder);
            CreatePath(TempFolder);

            Template = template;
        }

        public static string DataFolder { get; }
        public static string ConfigFolder { get; }
        public static string TempFolder { get; }

        public static void Save()
        {
            using var oStream = new StreamWriter(SettingsFile);
            File.WriteAllText(SettingsFile, string.Empty);
            var xmlParser = new XmlSerializer(typeof(SettingsFileXmlTemplate));
            xmlParser.Serialize(oStream, Template);

            oStream.Close();
        }
    }

    public class SettingsFileXmlTemplate
    {
        public SettingsFileXmlTemplate()
        {
#if WINDOWS
            DataFolder = $"C:/Users/{Environment.UserName}/Documents/";
            ConfigFolder = $"C:/Users/{Environment.UserName}/AppData/PMConfig/";
            TempFolder = $"C:/Users/{Environment.UserName}/AppData/Local/Temp/";
#endif
#if LINUX
            DataFolder = $"/home/{Environment.UserName}/Documents/";
            ConfigFolder = $"/home/{Environment.UserName}/.config/PMConfig/";
            TempFolder = "/tmp/";
#endif
        }

        [XmlElement(ElementName = "SpreadsheetUploadFolder")]
        public string DataFolder { get; set; }

        [XmlElement(ElementName = "ConfigurationFolder")]
        public string ConfigFolder { get; set; }

        [XmlElement(ElementName = "TemporaryDataFolder")]
        public string TempFolder { get; set; }
    }
}