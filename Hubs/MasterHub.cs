using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using BusMaster.Model;
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

    public override async Task OnConnectedAsync()
    {
      Console.WriteLine("in OnCnnectedAsync");
    }
    public override async Task OnDisconnectedAsync(Exception ex)
    {
      Console.WriteLine($"Disconnect: connectionid: {Context.ConnectionId}");
      //Console.WriteLine($"--> in OnDisconnectedAsync: {ex.Message} <--");
    }
    public async Task Login(string name, List<string> groups)
    {
      name = name.ToLower();
      var rigConf = RigConf.Instance;
      Console.WriteLine($"in login: {name}");
      var busConf = new BusConfigurationDB();
      busConf.Id = 20;
      busConf.Name = name;
      busConf.Configuration = JsonSerializer.Serialize(rigConf);


 
      foreach (var group in groups)
      {
        Console.WriteLine($"in groups: {group}");
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, group);
      }
      if (name == "control")
      {
        var busPacket = new UiInfoPacketModel();
        await Clients.Caller.SendAsync("InfoPacket", busPacket);
        return;
      }

      await this.Groups.AddToGroupAsync(this.Context.ConnectionId, name);
      await Clients.Caller.SendAsync("ReceiveConfiguration", busConf);
      return;
    }
    public async Task RadioStateChange(RigState state)
    {
      await Clients.Group("RadioStateChange").SendAsync("state", state);
      return;
    }
    public async Task SetConfiguration(string busName, BusConfigurationDB config)
    {


    }
    public async Task<List<BusConfigurationDB>> GetListOfBusesConfig()
    {
      var busList = new List<BusConfigurationDB>();
      return busList;
    }
  }
}
