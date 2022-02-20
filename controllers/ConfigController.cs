using System.IO;
using System.Xml.Serialization;

namespace PowerMonitor.controllers;

public class ConfigController
{
    private static readonly string ConfigPath = App.SettingsPath + "config.xml";
    public readonly ApplicationConfig? AppConfig; 

    public class ApplicationConfig
    {
        public bool LocalLoginCheck { get; set; }
        public bool LocalPlotData { get; set; }

        public ApplicationConfig()
        {
            LocalLoginCheck = true;
            LocalPlotData = true;
        }
        
    }

    public ConfigController()
    {
        if (File.Exists(ConfigPath))
        {
            StreamReader stream = new StreamReader(ConfigPath);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ApplicationConfig));
            AppConfig = xmlSerializer.Deserialize(stream) as ApplicationConfig;
            stream.Close();
        }
        else
        {
            AppConfig = new ApplicationConfig();
            StreamWriter stream = new StreamWriter(ConfigPath);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ApplicationConfig));
            xmlSerializer.Serialize(stream, AppConfig);

        }
    }
}