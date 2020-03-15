using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusMaster.Hubs
{
  public class MasterHub : Hub
  {
    public async Task LoginIn()
    {
      Console.WriteLine("in login");
    }
  }
}
