using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CoreHambusCommonLibrary.DataLib;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CoreHambusCommonLibrary.Services
{
  public class ActiveBusesService 
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Configuration { get; set; }
    public List<string> Ports { get; set; } = new List<string>();

    public ActiveBusesService()
    {

    }
  }
}
