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
    public static List<int> Complexes { get; set; }

    public static bool AdminStatus { get; set; }

    public static UserInfoCollection Users { get; set; }

    public static void InitLoginService()
    {
        // if no server validation is present.
        // this differs in build stage, so 
        // this is more like a debugging way

        if (SPath.ServerOn)
        {
            Users = new UserInfoCollection();
            return;
        }

        if (File.Exists(LoginsFile))
        {
            Shared.Logger!.Log(LogLevel.Info, "initiated login service succesfully");
            var stream = new StreamReader(LoginsFile);
            ParseHandler(stream.BaseStream, false);
        }
        else
        {
            Shared.Logger!.Log(LogLevel.Info, "login service not initiated");
            EnterDefault();
        }
    }

    public static bool LogIn(string login, string password)
    {
        if (SPath.ServerOn)
        {
            var res = NetworkService.LogIn(login, password).Result;
            if (res is null) return false;

            Complexes = res.Complexes.ToList();

            CurrentUser = login;
            CurrentPassword = password;
            AdminStatus = res.IsAdmin;

            UpdateUserList();


            return true;
        }

        foreach (var match in Users.UserInfoList!)
        {
            Shared.Logger!.Log(LogLevel.Info, $"checking {match.Name} with pas = {match.Password}");
            if (match.Password != null && match.Name != null && match.Name.Equals(login) &&
                match.Password.Equals(password))
            {
                CurrentPassword = match.Password;
                CurrentUser = match.Name;
                AdminStatus = match.IsAdmin;

                Complexes = match.Restrictions!.Select(int.Parse).ToList();

                return true;
            }
        }

        return false;
    }

    // handling for async parsing
    private static async void ParseHandler(Stream callingStream, bool write)
    {
        var res = await ParseLoginsAsync(callingStream, write);
        if (res) return;
        Shared.Logger!.Log(LogLevel.Info, "parse handler initiated");
        EnterDefault();
    }

    // async parsing
    private static async Task<bool> ParseLoginsAsync(Stream stream, bool write = false)
    {
        Shared.Logger!.Log(LogLevel.Info, "acync parcing of logins...");

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
                var readTask = Task.Run(() => Users = (xmlSerializer.Deserialize(stream) as UserInfoCollection)!);
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
        Shared.Logger!.Log(LogLevel.Info, "entering default...");

        var admin = new UserInfo("admin", "password", new List<string>(), true);
        var userColl = new UserInfoCollection {UserInfoList = new[] {admin}};
        Users = userColl;
    }

    // update logins file
    public static void UpdateLogins()
    {
        if (SPath.ServerOn) return;

        Shared.Logger!.Log(LogLevel.Info, "writing logins...");

        File.WriteAllText(LoginsFile, string.Empty);
        var stream = new StreamWriter(LoginsFile);
        ParseHandler(stream.BaseStream, true);
    }

    public static void UpdateUserList()
    {
        if (AdminStatus)
        {
            var result = NetworkService.GetUsers().Result;
            Shared.Logger.Log(LogLevel.Info, $"Getting users finished with {result} status");
        }
    }

    public static void AddUser(UserInfo user)
    {
        Shared.Logger!.Log(LogLevel.Info, "adding user...");

        if (SPath.ServerOn) NetworkService.CreateUserAsync(user);
        Users.UserInfoList = Users.UserInfoList.Append(user).ToArray();
    }

    public static void UpdateUser(UserInfo user, int index)
    {
        Shared.Logger!.Log(LogLevel.Info, "updating user...");

        if (!user.Name!.Equals("empty") && !user.Password!.Equals("empty"))
        {
            if (SPath.ServerOn) NetworkService.UpdateUserAsync(Users.UserInfoList[index], user);

            var infoSave = new UserInfo
            {
                Name = user.Name,
                Password = user.Password,
                IsAdmin = user.IsAdmin,
                Restrictions = user.Restrictions
            };

            Users.UserInfoList[index] = infoSave;
        }
    }

    public static void DeleteUser(int index)
    {
        Shared.Logger!.Log(LogLevel.Info, "deleting user...");
        if (SPath.ServerOn) NetworkService.DeleteUserAsync(Users.UserInfoList[index]);
        Users.UserInfoList[index] = new UserInfo();
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