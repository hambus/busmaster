using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreHambusCommonLibrary.DataLib;
using CoreHambusCommonLibrary.Services;
using HambusCommonLibrary;
using Microsoft.AspNetCore.SignalR;

namespace BusMaster.Hubs
{
  public class MasterHub : Hub
  {
    public IGlobalDataService GlobalData { get; set; }
    public MasterHub(IGlobalDataService globalDb)
    {
      GlobalData = globalDb;
    }
    public async Task Login(string name, List<string> groups)
    {
      Console.WriteLine($"in login: {name}");
      var busConf = new BusConfigurationDB();
      busConf.Id = 20;
      busConf.Configuration = "This should be a json";
      foreach (var group in groups)
      {
        Console.WriteLine($"in groups: {group}");
      }
      //await Groups.AddToGroupAsync(Context.ConnectionId, group);
      await Clients.Caller.SendAsync("ReceiveConfigation", busConf);
      return;
    }
    public async Task RadioStateChange(RigState state)
    {
      await Clients.Group("RadioStateChange").SendAsync("state", state);
      return;
    }
    public async Task SetConfiguration(string busName, BusConfigurationDB config)
    {
      var bConf = new BusConfigurationDB();

    }
  }
}
