﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisController.Models;
internal class RedisDataBaseConfiguration
{
    public RedisDataBaseConfiguration(string identifier, string host, string port)
    {
        DataBaseID = identifier;
        DataBaseHost = host;
        DataBasePort = port;
        DataBaseLastConnection = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    }

    public string DataBaseID {
        get;
        private set; 
    }

    public string DataBaseHost
    {
        get;
        private set;
    }

    public string DataBasePort
    {
        get;
        private set;
    }

    public DateOnly DataBaseLastConnection
    {
        get;
        private set;
    }
}

