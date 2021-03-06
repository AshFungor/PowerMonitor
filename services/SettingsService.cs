using System;
using System.IO;
using System.Xml.Serialization;
using ExtremelySimpleLogger;

namespace PowerMonitor.services;

public static class SettingsService
{
    private static readonly string SettingsFile = App.Path + "settings.xml";

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
                try
                {
                    Shared.Logger!.Log(LogLevel.Info, "reading settings...");

                    using var iStream = new StreamReader(SettingsFile);
                    var xmlParser = new XmlSerializer(typeof(SettingsFileXmlTemplate));
                    template = (SettingsFileXmlTemplate) xmlParser.Deserialize(iStream)!;

                    iStream.Close();
                }
                catch (Exception e)
                {
                    Shared.Logger!.Log(LogLevel.Error, $"reading settings unsuccessful, ex raised: {e.Message}");
                    Shared.Logger!.Log(LogLevel.Info, "creating settings file");

                    template = new SettingsFileXmlTemplate();
                }
            }
            else
            {
                Shared.Logger!.Log(LogLevel.Info, "creating settings file");

                template = new SettingsFileXmlTemplate();

                Shared.Logger!.Log(LogLevel.Info, "setting were created.");
            }

            DataFolder = template.DataFolder;
            ConfigFolder = template.ConfigFolder;
            TempFolder = template.TempFolder;
            ServerOn = template.ServerOn;
            ServerAddress = template.ServerAddress;

            CreatePath(DataFolder);
            CreatePath(ConfigFolder);
            CreatePath(TempFolder);

            Template = template;
        }

        // App settings, available to all app objects
        // all settings must be static and readonly,
        // hence only user is eligible to change them.
        public static string DataFolder { get; }
        public static string ConfigFolder { get; }
        public static string TempFolder { get; }
        public static bool ServerOn { get; }
        public static string ServerAddress { get; }

        public static void Save()
        {
            try
            {
                Shared.Logger!.Log(LogLevel.Info, "saving settings file...");
                File.WriteAllText(SettingsFile, string.Empty);
                using var oStream = new StreamWriter(SettingsFile);
                var xmlParser = new XmlSerializer(typeof(SettingsFileXmlTemplate));
                xmlParser.Serialize(oStream, Template);

                oStream.Close();

                Shared.Logger!.Log(LogLevel.Info, "settings were saved.");
            }
            catch (Exception e)
            {
                Shared.Logger!.Log(LogLevel.Error, $"saving settings unsuccesful, ex raised: {e.Message}");
            }
        }
    }

    public class SettingsFileXmlTemplate
    {
        public SettingsFileXmlTemplate()
        {
            // defaults may vary across build configurations,
            // yet some are independent from platform.
#if WINDOWS
            DataFolder = $"C:/Users/{Environment.UserName}/Documents/PM/";
            ConfigFolder = $"C:/Users/{Environment.UserName}/Documents/PM/config/";
            TempFolder = $"C:/Users/{Environment.UserName}/Documents/PM/tmp/";
#endif
#if LINUX
            DataFolder = $"/home/{Environment.UserName}/Documents/PM/";
            ConfigFolder = $"/home/{Environment.UserName}/Documents/PM/config/";
            TempFolder = $"/home/{Environment.UserName}/Documents/PM/tmp/";
#endif
            ServerOn = false;
            ServerAddress = string.Empty;
        }

        // xml elements possess more 
        // understandable name than class
        // members.
        [XmlElement(ElementName = "SpreadsheetUploadFolder")]
        public string DataFolder { get; set; }

        [XmlElement(ElementName = "ConfigurationFolder")]
        public string ConfigFolder { get; set; }

        [XmlElement(ElementName = "TemporaryDataFolder")]
        public string TempFolder { get; set; }

        [XmlElement(ElementName = "ServerConnectionPossible")]
        public bool ServerOn { get; set; }

        [XmlElement(ElementName = "ServerAddress")]
        public string ServerAddress { get; set; }
    }
}