using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ExtremelySimpleLogger;
using SPath = PowerMonitor.services.SettingsService.Settings;

namespace PowerMonitor.services;

// login controller
public static class LoginService
{
    private static readonly string LoginsFile = SPath.ConfigFolder + ".logins.xml";

    public static string CurrentUser { get; set; }
    public static string CurrentPassword { get; set; }

    public static UserInfoCollection Users { get; set; }

    public static void InitLoginService()
    {
        // if no server validation is present.
        // this differs in build stage, so 
        // this is more like a debugging way

        if (File.Exists(LoginsFile))
        {
            var stream = new StreamReader(LoginsFile);
            ParseHandler(stream.BaseStream, false);
        }
        else
        {
            EnterDefault();
        }
    }

    // handling for async parsing
    private static async void ParseHandler(Stream callingStream, bool write)
    {
        var res = await ParseLoginsAsync(callingStream, write);
        if (res) return;

        EnterDefault();
    }

    // async parsing
    private static async Task<bool> ParseLoginsAsync(Stream stream, bool write = false)
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
        Users!.UserInfoList = Users!.UserInfoList!
            .Where(el => !el.Name!.Equals("empty") || !el.Password!.Equals("empty"))
            .ToArray();
        var writeTask = Task.Run(() => xmlSerializer.Serialize(stream, Users));
        await writeTask;
        return writeTask.IsCompleted;
    }

    // not to break app on every parse fail, there always should be a way out
    private static void EnterDefault()
    {
        var admin = new UserInfo("admin", "password", new List<string>(), true);
        var userColl = new UserInfoCollection {UserInfoList = new[] {admin}};
        Users = userColl;
    }

    // update logins file
    public static void UpdateLogins()
    {
        Shared.Logger!.Log(LogLevel.Info, "writing logins...");

        File.WriteAllText(LoginsFile, string.Empty);
        var stream = new StreamWriter(LoginsFile);
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
            Name = "empty";
            Password = "empty";
            Restrictions = new List<string>();
            IsAdmin = false;
        }

        [XmlArray] public List<string>? Restrictions { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public bool IsAdmin { get; set; }
    }

    // collection of users
    public sealed class UserInfoCollection
    {
        [XmlArray] public UserInfo[] UserInfoList { get; set; }
    }
}