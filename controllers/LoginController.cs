using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using SimpleLogger;

namespace PowerMonitor.controllers;

public sealed class LoginController
{
    private static string logins_file = ".logins.xml";
    public UserInfoCollection Users { get; set; }

    public sealed class UserInfo
    {
        [XmlArrayAttribute] public List<string>? Restrictions { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }

        public UserInfo(string name, string password, List<string> rests)
        {
            Name = name;
            Password = password;
            Restrictions = rests;
        }

        public UserInfo()
        {
            Name = null;
            Password = null;
            Restrictions = null;
        }
    }

    public sealed class UserInfoCollection
    {
        [XmlArrayAttribute] public UserInfo[]? UserInfoList { get; set; }
    }

    public LoginController()
    {
        Logger.Log<LoginController>("creating login controller instance, checking for logins file...");
        var xmlSerializer = new XmlSerializer(typeof(UserInfoCollection));

        if (File.Exists(App.SettingsPath + logins_file))
        {
            Logger.Log<LoginController>("logins file found");
            var stream = new StreamReader(App.SettingsPath + logins_file);

            UserInfoCollection? userColl = null;
            Logger.Log<LoginController>("trying to parse logins file");
            try
            {
                userColl = xmlSerializer.Deserialize(stream) as UserInfoCollection;
            }
            catch (InvalidOperationException ex)
            {
                Logger.Log<LoginController>(Logger.Level.Error,
                    $"could not parse logins file: {ex.Message}, default session applied");
                EnterDefault();
                return;
            }

            Users = userColl;
        }
        else
        {
            Logger.Log<LoginController>(Logger.Level.Error, "could not find logins file, default session applied");
            EnterDefault();
        }
    }

    private void EnterDefault()
    {
        var admin = new UserInfo("admin", "password", new List<string>());
        var userColl = new UserInfoCollection() {UserInfoList = new[] {admin}};
        Users = userColl;
    }

    public void UpdateLogins()
    {
        Logger.Log<LoginController>("writing logins...");

        File.WriteAllText(App.SettingsPath + logins_file, string.Empty);
        var stream = new StreamWriter(App.SettingsPath + logins_file);
        var xmlSerializer = new XmlSerializer(typeof(UserInfoCollection));
        xmlSerializer.Serialize(stream, Users);
    }
}