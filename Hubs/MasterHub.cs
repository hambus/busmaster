using CoreHambusCommonLibrary.Services;
using HambusCommonLibrary;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

      foreach (var group in groups)
      {
        Console.WriteLine($"in groups: {group}");
      }
      //await Groups.AddToGroupAsync(Context.ConnectionId, group);
      await Clients.All.SendAsync("loginResponse", $"Login with group: {name}");
      return;
    }
    public async Task RadioStateChange(RigState state)
    {
      await Clients.Group("RadioStateChange").SendAsync("state", state);
      return;
    }
  }
}
