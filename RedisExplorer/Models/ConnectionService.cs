﻿using StackExchange.Redis;
using System.Text.Json;

namespace RedisExplorer.Models;

public class ConnectionService
{
    public List<RedisDatabaseConfiguration> Configurations { get; private set; }
    public Dictionary<string, ConnectionMultiplexer> Connections { get; set; }

    private string FileWithConfigs { get; set; }

    public ConnectionService()
    {
        Connections = new Dictionary<string, ConnectionMultiplexer>();
        Configurations = new List<RedisDatabaseConfiguration>();
        
        var appDataDirectory = FileSystem.AppDataDirectory;
        var configurationsDirectory = Path.Combine(appDataDirectory, "Configurations");
        FileWithConfigs = Path.Combine(configurationsDirectory, "configs.json");

        if (!File.Exists(FileWithConfigs))
        {
            Directory.CreateDirectory(configurationsDirectory);
            var file = File.Create(FileWithConfigs);
            file.Close();
            UpdateConfigsAsync();
        }
        else
        {
            string jsonString = File.ReadAllText(FileWithConfigs);
            Configurations = JsonSerializer.Deserialize<List<RedisDatabaseConfiguration>>(jsonString);
            
        }
    }

    public void AddNewConnection(string dataBaseID, ConnectionMultiplexer newConnection)
    {
        Connections.Add(dataBaseID, newConnection);
    }

    public bool WasConnected(string dataBaseID)
    {
        return Connections.ContainsKey(dataBaseID);
    }


    public async Task<ConnectionMultiplexer> CreateConnectionAsync(string dataBaseID)
    {
        var config = Configurations.Find((conf) => conf.DatabaseID == dataBaseID);

        string connectionConfig = $"{config.DatabaseHost}:{config.DatabasePort}, password = {config.DatabasePassword}, allowAdmin=true";
        return await ConnectionMultiplexer.ConnectAsync(connectionConfig);
    }


    public async void UpdateConfigsAsync()
    {
        using FileStream createStream = File.Create(FileWithConfigs);
        await JsonSerializer.SerializeAsync(createStream, Configurations);

    }
}

