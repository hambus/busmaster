using System;
using System.Data;
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
            CreateTable(conn);
            QueryApps(conn, "master");


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

    private void QueryApps(IDbConnection conn, string name)
    {

      var cmd = conn.CreateCommand();
      cmd.CommandText = $"SELECT name FROM master_conf where name = '{name}'";

      var reader = cmd.ExecuteReader();

      if (!reader.Read())
      {
        CreateInitalEntryForMasterBus(cmd, reader);
      }
      else
      {
        UpdateBusEntry(cmd, reader, null);
      }
    }

    private void UpdateBusEntry(IDbCommand cmd, IDataReader reader, BusConfigurationDB? conf)
    {
      throw new NotImplementedException();
    }
    private void InsertBusEntry(IDbCommand cmd, IDataReader reader, BusConfigurationDB conf)
    {
      throw new NotImplementedException();
    }

    private void CreateInitalEntryForMasterBus(IDbCommand cmd, IDataReader reader)
    {
      if (this.Port == null) this.Port = 7300;

      var conf = "{}";
      reader.Close();
      cmd.CommandText = $"insert into master_conf ( version, name, configuration) values ( 1.0,  \"{Name}\", \"{conf}\")";
      cmd.ExecuteNonQuery();
    }

    private void CreateTable(IDbConnection conn)
    {
      var createCmd = "CREATE table IF NOT EXISTS [master_conf] (" +
        "[id]  INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
        "[name]  TEXT NOT NULL, " +
        "[configuration] TEXT, " +
        "[version]  NUMERIC NOT NULL DEFAULT 1 " +
         ")";
      Console.WriteLine(createCmd);
      conn.Execute(createCmd);
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
