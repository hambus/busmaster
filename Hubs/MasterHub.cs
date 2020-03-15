using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusMaster.Hubs
{
  public class MasterHub : Hub
  {
    public async Task Login(string group)
    {

      Console.WriteLine($"in login: {group}");
      await Clients.All.SendAsync("loginResponse", $"Login with group: {group}");
      return;
    }
  }
}
