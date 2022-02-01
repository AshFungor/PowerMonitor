using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace PowerMonitor.controllers;

public sealed class LoginController
{
    private static string logins_file = ".logins.xml";
    public UserInfoCollection Users { get; set; }

    public sealed class UserInfo
    {
        public string Name { get; set; }
        [XmlArrayAttribute]
        public List<string> Restrictions { get; set; }
        public string Password { get; set; }

        public UserInfo(string name, string password, List<string> rests)
        {
            Name = name;
            Password = password;
            Restrictions = rests;
        }
        public  UserInfo() {}
    }

    public sealed class UserInfoCollection
    {
        [XmlArrayAttribute]
        public UserInfo[] UserInfoList { get; set; }
    }

    public LoginController()
    {
        if (File.Exists(App.SettingsPath + logins_file))
        {
            StreamReader istream = new StreamReader(App.SettingsPath + logins_file);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserInfoCollection));
            UserInfoCollection user_coll = null;
            try
            {
                user_coll = xmlSerializer.Deserialize(istream) as UserInfoCollection;
            }
            catch (XmlException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            Users = user_coll;
        }
        else
        {
            StreamWriter ostream = new StreamWriter(App.SettingsPath + logins_file);
            UserInfo admin = new UserInfo("admin", "password", new List<string>());
            XmlSerializer xml_serializer = new XmlSerializer(typeof(UserInfoCollection));
            UserInfoCollection user_coll = new UserInfoCollection() {UserInfoList = new[] {admin}};
            xml_serializer.Serialize(ostream, user_coll);
            Users = user_coll;
        }


    }

    public void UpdateLogins()
    {
        File.Delete(App.SettingsPath + logins_file);
        StreamWriter ostream = new StreamWriter(App.SettingsPath + logins_file);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserInfoCollection));
        xmlSerializer.Serialize(ostream, Users);
    }


}