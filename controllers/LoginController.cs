using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Threading.Tasks;
using ExtremelySimpleLogger;

namespace PowerMonitor.controllers;

public sealed class LoginController
{
    private static string logins_file = ".logins.xml";

    public UserInfoCollection? Users { get; set; }

    // classes for parsing
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
            var readTask = Task.Run(() => Users = xmlSerializer.Deserialize(stream) as UserInfoCollection);
            await readTask;
            return readTask.IsCompleted;
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
        var admin = new UserInfo("admin", "password", new List<string>());
        var userColl = new UserInfoCollection() {UserInfoList = new[] {admin}};
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
}