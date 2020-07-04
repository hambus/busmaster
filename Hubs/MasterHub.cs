﻿using System;
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

    //public override async Task OnConnectedAsync()
    //{
    //  Console.WriteLine("in OnCnnectedAsync");
    //}
    //public override async Task OnDisconnectedAsync(Exception ex)
    //{
    //  Console.WriteLine($"Disconnect: connection-id: {Context.ConnectionId}");

    //}
    public async Task Login(string name, List<string> groups, List<string> ports)
    {
      name = name.ToLower();


      BusConfigurationDB? busConf = null;// = new BusConfigurationDB();

      ActiveBuses.Ports = ports;
      ActiveBuses.Name = name;
      ActiveBuses.Configuration = "{}";

      var conf = await GetBusByName(name);
      var confs = await GlobalData.GetBusConfigList();


      if (name == "control")
      {
        var busPacket = new UiInfoPacketModel();
        busPacket.BusesInDb = confs;

        await Clients.Caller.SendAsync("InfoPacket", busPacket);
        return;
      }
      if (conf != null)
      {
        await setGroups(groups);
        await Clients.Caller.SendAsync("ReceiveConfiguration", conf);
      }
      else
      {
        var errorReport = new HamBusError();
        errorReport.ErrorNum = HamBusErrorNum.NoConfigure;
        errorReport.Message = $"No configuration found:  Please go to http://localhost/7300";
        await Clients.Caller.SendAsync("ErrorReport", errorReport);
      }

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
      await Clients.Group("RadioStateChange").SendAsync("state", state);
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
        await GlobalData.InsertBusEntry(config);
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
      return busList;
    }
  }
}
