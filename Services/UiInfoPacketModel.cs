using System.Collections.Generic;
using BusMaster;
using CoreHambusCommonLibrary.DataLib;
using CoreHambusCommonLibrary.Model;
using CoreHambusCommonLibrary.Services;
using HamBusCommonCore.Model;

namespace CoreHambusCommonLibrary.Services
{
  public class UiInfoPacketModel : HamBusBase
  {
    public List<ActiveBusesModel> ActiveBuses { get; set; } = new List<ActiveBusesModel>();
    public List<BusConfigurationDB> BusesInDb { get; set; } = new List<BusConfigurationDB>();
  }
}
