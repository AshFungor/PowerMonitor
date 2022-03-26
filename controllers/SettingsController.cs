using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PowerMonitor.controllers;

public static class SettingsController
{
    private static readonly string SettingsFile = App.SettingsPath + "settings.xml";


    public static class Settings
    {
        public static string DataFolder { get; private set;  } 
        public static string ConfigFolder { get; private set;  }
        public static string TempFolder { get; private set;  }
        private static readonly SettingsFileXmlTemplate Template;
        

        static Settings()
        {
            SettingsFileXmlTemplate template;
            if (SettingsPresent())
            {
                using var iStream = new StreamReader(SettingsFile);
                var xmlParser = new XmlSerializer(typeof(SettingsFileXmlTemplate));
                template = (SettingsFileXmlTemplate) xmlParser.Deserialize(iStream)!;
            }
            else template = new SettingsFileXmlTemplate(false);

            DataFolder = template.DataFolder;
            ConfigFolder = template.ConfigFolder;
            TempFolder = template.TempFolder;
            
            CreatePath(DataFolder);
            CreatePath(ConfigFolder);
            CreatePath(TempFolder);

            Template = template;
        }

        public static void Save()
        {
            using var oStream = new StreamWriter(SettingsFile);
            File.WriteAllText(SettingsFile, String.Empty);
            var xmlParser = new XmlSerializer(typeof(SettingsFileXmlTemplate));
            xmlParser.Serialize(oStream, Template);
        }





    }

    public class SettingsFileXmlTemplate
    {
        [XmlElement(ElementName = "SpreadsheetUploadFolder")]
        public string DataFolder { get; set; } = string.Empty;
        [XmlElement(ElementName = "ConfigurationFolder")]
        public string ConfigFolder { get; set; } = string.Empty;
        [XmlElement(ElementName = "TemporaryDataFolder")]
        public string TempFolder { get; set; } = string.Empty;
        
        public SettingsFileXmlTemplate(bool @default = false)
        {
            
            if (@default) return;
#if WINDOWS
            DataFolder = $"C:/Users/{Environment.UserName}/Documents/";
            ConfigFolder = $"C:/Users/{Environment.UserName}/AppData/PMConfig/";
            TempFolder = $"C:/Users/{Environment.UserName}/AppData/Local/Temp/";
#endif
#if LINUX
            DataFolder = $"/home/{Environment.UserName}/Documents/";
            ConfigFolder = $"/home/{Environment.UserName}/.config/PMConfig/";
            TempFolder = $"/tmp/";
#endif

        }

        public SettingsFileXmlTemplate() { }

    }

    private static bool SettingsPresent()
    {
        return File.Exists(SettingsFile);
    }

    private static void CreatePath(string path)
    {
        string pPath = "/";
        foreach (var part in path.Split('/').Skip(1))
        {
            pPath += path;
            if (Directory.Exists(pPath))
            {
                pPath += "/";
                continue;
            }
            if (File.Exists(pPath))
                return;
            Directory.CreateDirectory(pPath);
            pPath += "/";
            

        }
    }



}