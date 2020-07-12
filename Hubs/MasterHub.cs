using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusMaster.Model;
using CoreHambusCommonLibrary.DataLib;
using CoreHambusCommonLibrary.Model;
using CoreHambusCommonLibrary.Services;
using HamBusCommmonCore;
using Microsoft.AspNetCore.SignalR;

namespace BusMaster.Hubs
{
  public class MasterHub : Hub
  {
    public IGlobalDataService GlobalData { get; set; }
    public ActiveBusesService ActiveBuses { get; set; }
    
    public MasterHub(IGlobalDataService globalDb, ActiveBusesService activesBuses)
    {
      GlobalData = globalDb;
      this.ActiveBuses = activesBuses;
    }

    public override async Task OnConnectedAsync()
    {
      await Task.Delay(0);
      Console.WriteLine("in OnCnnectedAsync");
    }
    public override async Task OnDisconnectedAsync(Exception ex)
    {
      await Task.Delay(0);
      Console.WriteLine($"Disconnect: connection-id: {Context.ConnectionId}");
    }
    public async Task Login(string name, List<string> groups, List<string> ports)
    {
      name = name.ToLower();

      ActiveBuses.Ports = ports;
      ActiveBuses.Name = name;
      ActiveBuses.Configuration = "{}";

      var currentBusConf = await GetBusByName(name);
      Console.WriteLine(currentBusConf!.Configuration);
      var confs = await GlobalData.GetBusConfigList();


      if (name == "control")
      {

        await CreateResponseForControl(confs);
      }

      if (currentBusConf != null)
      {
        Console.WriteLine(currentBusConf.Configuration);
        await setGroups(groups);
        await Clients.Caller.SendAsync("ReceiveConfiguration", currentBusConf);
      }
      else
      {
        await CreateResponseForBuses();
      }

      return;
    }

    private async Task CreateResponseForBuses()
    {
      var errorReport = new HamBusError();
      errorReport.ErrorNum = HamBusErrorNum.NoConfigure;
      errorReport.Message = $"No configuration found:  Please go to http://localhost/7300";
      await Clients.Caller.SendAsync("ErrorReport", errorReport);
    }

    private async Task CreateResponseForControl(List<BusConfigurationDB> confs)
    {
      var groupList = new List<string>();
      groupList.Add("UI");
      var busPacket = new UiInfoPacketModel();
      busPacket.BusesInDb = confs;
      await setGroups(groupList);
      await Clients.Caller.SendAsync("InfoPacket", busPacket);
      return;
    }

    private async Task setGroups(List<string> groups)
    {
      foreach (var group in groups)
      {
        Console.WriteLine($"in groups: {group}");
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, group);
      }
    }

    public async Task RadioStateChange(RigState state)
    {
      Console.WriteLine($"State change {state.Freq}");
      await Clients.Group("radio").SendAsync("state", state);
      return;
    }
    public async Task SaveConfiguration(string busName , BusConfigurationDB config)
    {
      Console.WriteLine(busName);
      if (config?.Id != null)
      {
        await GlobalData.UpdateBusEntry(busName, config);
      }
      else
      {
        await GlobalData.InsertBusEntry(config!);
      }
    }
    public async Task<BusConfigurationDB?> GetBusByName(string busName)
    {
      var rc = await GlobalData.QueryBusByName(busName);
      return rc;
    }
    public async Task<List<BusConfigurationDB>> GetListOfBuses()
    {
      var busList = new List<BusConfigurationDB>();
      await Task.Delay(0);
      return busList;
    }
  }
}
