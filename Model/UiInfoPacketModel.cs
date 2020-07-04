using System.Collections.Generic;
using CoreHambusCommonLibrary.DataLib;

namespace BusMaster.Model
{
  public class UiInfoPacketModel
  {
    public List<ActiveBusesModel> ActiveBuses { get; set; } = new List<ActiveBusesModel>();
    public List<BusConfigurationDB> BusesInDb { get; set; } = new List<BusConfigurationDB>();
  }
}
