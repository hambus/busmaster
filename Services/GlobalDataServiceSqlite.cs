using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CoreHambusCommonLibrary.DataLib;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CoreHambusCommonLibrary.Services
{
  public class GlobalDataServiceSqlite : IGlobalDataService
  {
    public int Id { get; set; }
    public string Name { get; } = "master";
    public double Version { get; set; } = 1.0;
    public string Host { get; set; } = "";
    public int? Port { get; set; }
    public string? ConnString { get; set; }
    public string? Configuration { get; set; }
    public BusInit busInit { get; set; } = new BusInit();



    #region singleton
    private static readonly object padlock = new object();
    private static GlobalDataServiceSqlite? instance = null;
    public static GlobalDataServiceSqlite Instance
    {
      get
      {
        lock (padlock)
        {
          if (instance == null)
          {
            instance = new GlobalDataServiceSqlite();
          }
          return instance;
        }
      }
    }
    #endregion

    public GlobalDataServiceSqlite()
    {
      InitDB();
    }

    public void InitDB()
    {

      BuildConnectString();
      try
      {
        using (IDbConnection conn = new SqliteConnection(ConnString))
        {
          try
          {
            conn.Open();
            if (!DoesTableExist("master_conf"))
            {
              CreateTable(conn);
              Console.WriteLine("created table");
            }
            else
              Console.WriteLine("Table exist");
          }
          catch (Exception ee)
          {
            Console.WriteLine($"InitDB {ee.Message}");
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);

      }
    }

    public async Task<BusConfigurationDB?> QueryBusByName(string name)
    {
      BusConfigurationDB? busConf = null;

      using (var conn = new SqliteConnection(ConnString))
      {
        try
        {
          conn.Open();
          var commandText = $"SELECT name FROM master_conf where name = '{name.ToLower()}'";
          var cmd = new SqliteCommand(commandText, conn);
          var reader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);
          if (reader.HasRows)
          {
            busConf = new BusConfigurationDB();
            busConf.Id = (int)reader["id"];
            busConf.Name = (string)reader["name"];
            busConf.Version = (int)reader["version"];
            busConf.Configuration = (string)reader["configuration"];
          }

          return busConf;
        }
        catch (Exception e)
        {

          Console.WriteLine($"QueryBusByName: {e.Message}");
          return null;
        }
      }
    }

    public async Task UpdateBusEntry(string? name, BusConfigurationDB? conf)
    {
      using (var conn = new SqliteConnection(ConnString))
      {
        conn.Open();
        using (var transaction = conn.BeginTransaction())
        {
          var cmd = conn.CreateCommand();
          cmd.CommandText = $"update into master_conf ( version, name, configuration) values " +
            $"( 1.0,  '{conf!.Name.ToLower()}', '{conf.Configuration}') where id = '{conf.Id}'";
          await cmd.ExecuteNonQueryAsync();
        }
      }
    }
    public async Task InsertBusEntry(BusConfigurationDB conf)
    {
      using (var conn = new SqliteConnection(ConnString))
      {
        conn.Open();

        using (var transaction = conn.BeginTransaction())
        {
          var cmd = conn.CreateCommand();
          //cmd.CommandText = $"insert into master_conf ( version, name, configuration) values ( 1.0,  \"{conf.Name}\", \"json({conf.Configuration})\")";
          cmd.CommandText = $"insert into master_conf ( version, name, configuration) values ( @version, @name, @conf)";
          cmd.Parameters.AddWithValue("@version", conf.Version);
          cmd.Parameters.AddWithValue("@name",conf.Name);
          cmd.Parameters.AddWithValue("@bustype",(int) conf.BusType);
          cmd.Parameters.AddWithValue("@conf", conf.Configuration);
          cmd.Prepare();
          //\"{conf.Name}\", \"json({conf.Configuration})\")";
          await cmd.ExecuteNonQueryAsync();
        }
      }
    }

    //private void CreateInitalEntryForMasterBus(IDbCommand cmd, IDataReader reader)
    //{
    //  if (this.Port == null) this.Port = 7300;

    //  var conf = "{}";
    //  reader.Close();
    //  using (IDbConnection conn = new SqliteConnection(ConnString))
    //  {
    //    using (var transaction = conn.BeginTransaction())
    //    {
    //      cmd.CommandText = $"insert into master_conf ( version, name, configuration) values ( 1.0,  \"{Name}\", \"{conf}\")";
    //      cmd.ExecuteNonQuery();
    //    }
    //  }
    //}

    private void CreateTable(IDbConnection conn)
    {
      var createCmd = "CREATE table IF NOT EXISTS [master_conf] (" +
        "[id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
        "[name] TEXT NOT NULL, " +
        "[configuration] TEXT, " +
        "[version] NUMERIC NOT NULL DEFAULT 1, " +
        "[bustype] NUMERIC NOT NULL DEFAULT 0 " +
         ")";
      Console.WriteLine(createCmd);
      conn.Execute(createCmd);
    }

    public async Task<List<BusConfigurationDB>> GetBusConfigList()
    {
      using (var conn = new SqliteConnection(ConnString))
      {
        conn.Open();
        var list = new List<BusConfigurationDB>();

        var commandText = $"SELECT id, name, configuration, version FROM master_conf";
        var cmd = new SqliteCommand(commandText, conn);
        //var reader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);
        using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.Default))
        {
          while (reader.Read())
          {
            var item = new BusConfigurationDB();
            item.Id = reader.GetInt32(0);
            item.Name = reader.GetString(1);
            item.Configuration = reader.GetString(2);
            item.Version = reader.GetInt32(3);
            list.Add(item);
            break;
          }
          return list;
        }
      }
    }

    private bool DoesTableExist(string tableName)
    {
      using (var conn = new SqliteConnection(ConnString))
      {
        conn.Open();
        bool rc = false;
        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'";

        using (var reader = cmd.ExecuteReader())
        {
          while (reader.Read())
          {
            var table = reader.GetString(0);
            rc = !string.IsNullOrWhiteSpace(table);
            break;
          }
          return rc;
        }
      }
    }


    private void BuildConnectString()
    {
      var source = busInit.DataFolder + "\\hambus.db";
      ConnString = $"data source={source}";
    }

    private string BuildQuery()
    {
      return "";
    }
  }
}
