using System.Data;
using Microsoft.Data.Sqlite;
using SimpleLogger;

namespace PowerMonitor.controllers;

public sealed class DbController
{
    private SqliteConnection _activeConnection;
    private static string _dbPath = App.SettingsPath + "database.db";
    
    
    public DbController()
    {
        Logger.Log<DbController>("initializing database controller");
        _activeConnection = new SqliteConnection($"DataSource={_dbPath}");
        _activeConnection.Open();
        if (_activeConnection.State != ConnectionState.Open)
        {
            Logger.Log<DbController>(Logger.Level.Error,"connection is failed to open");
            
            return;
        }
        
        // sample command
        SqliteCommand command = _activeConnection.CreateCommand();
        // command.CommandText = "SELECT title FROM albums";
        /*
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var name = reader.GetString(0);

            Logger.Log($"Hello, {name}!");
        }
        */
    }
    
    
    
}