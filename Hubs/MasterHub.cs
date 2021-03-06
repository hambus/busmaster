﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreHambusCommonLibrary.Model;
using CoreHambusCommonLibrary.Services;
using HamBusCommonCore.Model;
using HamBusCommonStd;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace BusMaster.Hubs
{
  public class MasterHub : Hub
  {
  
    public IGlobalDataService GlobalData { get; set; }
    private ActiveBusesService ActiveService { get; }
    private RigState globalState = new RigState();
    #region Setup
    public MasterHub(IGlobalDataService gd, ActiveBusesService ab)
    {
      GlobalData = gd;
      ActiveService = ab;
      globalState.Freq = 7777777;
      globalState.Mode = "USB";
      
    }

    public override async Task OnConnectedAsync()
    {
      await Task.Delay(0);
      Log.Debug("in OnCnnectedAsync");
    }
    public override async Task OnDisconnectedAsync(Exception ex)
    {
      await Task.Delay(0);
      Log.Debug($"Disconnect: connection-id: {Context.ConnectionId}");
      var removedBus = ActiveService.FindById(Context.ConnectionId);
      ActiveService.Remove(Context.ConnectionId);
      if (removedBus != null)
      {
        removedBus.IsActive = false;
        await SendActiveUpdate(removedBus);
      }
    }
    #endregion

    public async Task LockRig(LockModel locker)
    {
      await Clients.Group(locker.Name).SendAsync(SignalRCommands.LockRig, locker);
    }
    public async Task LockAck(LockModel locker)
    {
      await Clients.Group(SignalRGroups.Ui).SendAsync(SignalRCommands.LockRigAck, locker);
    }
    public async Task Login(string name, List<string> groups)
    {
      name = name.ToLower();


      var confs = await GlobalData.GetBusConfigList();

      if (name == "control")
        await SendResponseForControl(confs);
      else
      {
        groups.Add(name);
        var currentBusConf = await GetBusByName(name);
        if (currentBusConf == null)
        {
          Log.Warning("GetBusByName returned null on Login");
        }
        var newBus = new ActiveBusesModel
        {
          Name = name,
          ConnectionId = Context.ConnectionId,
          IsActive = true,
          Type = BusType.RigBus
        };

        if (newBus.State == null)
          newBus.State = new RigState();
        ActiveService.Add(newBus);

        if (currentBusConf!.BusType == BusType.VirtualRigBus)
        {
          Log.Warning("sending state to virtual rig");
          globalState.Name = name;
          await Clients.Caller.SendAsync(SignalRCommands.State, globalState);
        }

        await SendActiveUpdate(newBus);
        await SendResponseToBuses(groups, currentBusConf, name);
      }
      return;
    }
    private async Task SendActiveUpdate(ActiveBusesModel activeUpdate)
    {
      activeUpdate.IncSerial();
      await Clients.Group(SignalRGroups.Control).SendAsync(SignalRCommands.ActiveUpdate, activeUpdate);
    }
    private async Task SendResponseToBuses(List<string> groups, BusConfigurationDB? currentBusConf, 
      string name)
    {
      if (currentBusConf != null)
      {
        Log.Debug(currentBusConf.Configuration);
        await SetGroups(groups);
        currentBusConf.IncSerial();
        await Clients.Caller.SendAsync(SignalRCommands.ReceiveConfiguration, currentBusConf);
      }
      else
      {
        await NoConfigFoundForBus(name);
      }
    }

    private async Task NoConfigFoundForBus(string name)
    {
      var errorReport = new HamBusError
      {
        ErrorNum = HamBusErrorNum.NoConfigure,
        Message = $"No configuration found:  Please go to http://localhost/7300"
      };
      var newConf = new BusConfigurationDB
      {
        Name = name,
        Version = 1,
      };

      await GlobalData.InsertBusEntry(newConf);
      errorReport.IncSerial();
      await Clients.Caller.SendAsync(SignalRCommands.ErrorReport, errorReport);
    }

    private async Task SendResponseForControl(List<BusConfigurationDB> confs)
    {
      var groupList = new List<string>
      {
        SignalRGroups.Control
      };

      var infoPkt = new UiInfoPacketModel
      {
        BusesInDb = confs,
        ActiveBuses = ActiveService.ActiveBuses
      };
      await SetGroups(groupList);
      infoPkt.IncSerial();
      await Clients.Caller.SendAsync(SignalRCommands.InfoPacket, infoPkt);
      return;
    }

    private async Task SetGroups(List<string> groups)
    {
      foreach (var group in groups)
      {
        Log.Debug($"in groups: {group}");
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, group);
      }
    }

    public async Task RadioStateChange(RigState state)
    {
      
      state.IncSerial();
      globalState = state;
      Log.Debug("146: State change {@state} ", state);
      ActiveService.UpdateState(state);
      await Clients.Group(SignalRGroups.Control).SendAsync(SignalRCommands.State, state);
      await Clients.Group(SignalRGroups.Radio).SendAsync(SignalRCommands.State, state);


      return;
    }
    public async Task SaveConfiguration(string busName , BusConfigurationDB config)
    {
      Log.Debug(busName);
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
