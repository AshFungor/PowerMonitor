using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ExtremelySimpleLogger;

namespace PowerMonitor.controllers;

// login controller
public sealed class LoginController
{
    private static readonly string logins_file = ".logins.xml";

    public LoginController()
    {
        // if no server validation is present.
        // this differs in build stage, so 
        // this is more like a debugging way
#if !SERVER
        Shared.Logger!.Log(LogLevel.Info, "creating login controller instance, checking for logins file...");

        if (File.Exists(App.SettingsPath + logins_file))
        {
            Shared.Logger!.Log(LogLevel.Info, "logins file found");
            var stream = new StreamReader(App.SettingsPath + logins_file);

            Shared.Logger!.Log(LogLevel.Info, "trying to parse logins file");
            ParseHandler(stream.BaseStream, false);
        }
        else
        {
            Shared.Logger!.Log(LogLevel.Error, "could not find logins file, default session applied");
            EnterDefault();
        }
#endif
    }

    public UserInfoCollection? Users { get; set; }

    // handling for async parsing
    private async void ParseHandler(Stream callingStream, bool write)
    {
        var res = await ParseLoginsAsync(callingStream, write);
        if (res)
        {
            Shared.Logger!.Log(LogLevel.Info, "parse successful");
            return;
        }

        Shared.Logger!.Log(LogLevel.Warn, "parse unsuccessful");
        Shared.Logger!.Log(LogLevel.Warn, "entering default session");
        EnterDefault();
    }

    // async parsing
    private async Task<bool> ParseLoginsAsync(Stream stream, bool write = false)
    {
        var xmlSerializer = new XmlSerializer(typeof(UserInfoCollection));
        // parsing is done in a separate thread in case of huge
        // logins.xml file size, though in normal use this
        // would be obsolete.
        if (!write)
        {
            if (!stream.CanRead)
                return false;
            try
            {
                var readTask = Task.Run(() => Users = xmlSerializer.Deserialize(stream) as UserInfoCollection);
                await readTask;
                return readTask.IsCompleted;
            }
            catch (Exception e)
            {
                Shared.Logger!.Log(LogLevel.Error, e.Message);
                return false;
            }
        }

        if (!stream.CanWrite)
            return false;
        var writeTask = Task.Run(() => xmlSerializer.Serialize(stream, Users));
        await writeTask;
        return writeTask.IsCompleted;
    }

    // not to break app on every parse fail, there always should be a way out
    private void EnterDefault()
    {
        var admin = new UserInfo("admin", "password", new List<string>(), true);
        var userColl = new UserInfoCollection {UserInfoList = new[] {admin}};
        Users = userColl;
    }

    // update logins file
    public void UpdateLogins()
    {
        Shared.Logger!.Log(LogLevel.Info, "writing logins...");

        File.WriteAllText(App.SettingsPath + logins_file, string.Empty);
        var stream = new StreamWriter(App.SettingsPath + logins_file);
        ParseHandler(stream.BaseStream, true);
    }

    // classes for parsing
    public sealed class UserInfo
    {
        public UserInfo(string name, string password, List<string> rests, bool isAdmin)
        {
            Name = name;
            Password = password;
            Restrictions = rests;
            IsAdmin = isAdmin;
        }

        public UserInfo()
        {
            Name = null;
            Password = null;
            Restrictions = null;
            IsAdmin = false;
        }

        [XmlArrayAttribute] public List<string>? Restrictions { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public bool IsAdmin { get; set; }
    }

    // collection of users
    public sealed class UserInfoCollection
    {
        [XmlArrayAttribute] public UserInfo[]? UserInfoList { get; set; }
    }
}