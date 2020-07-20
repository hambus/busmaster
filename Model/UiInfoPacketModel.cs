using System.Collections.Generic;
using CoreHambusCommonLibrary.DataLib;
using HamBusCommonCore.Model;

namespace BusMaster.Model
{
  public class UiInfoPacketModel : HamBusBase
  {
    public List<ActiveBusesModel> ActiveBuses { get; set; } = new List<ActiveBusesModel>();
    public List<BusConfigurationDB> BusesInDb { get; set; } = new List<BusConfigurationDB>();
  }
}
