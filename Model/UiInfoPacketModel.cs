using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreHambusCommonLibrary.DataLib;

namespace BusMaster.Model
{
  public class UiInfoPacketModel
  {
    public List<ActiveBusesModel> ActiveBuses { get; set; } = new List<ActiveBusesModel>();
    public List<BusConfigurationDB> BusesInDb { get; set; } = new List<BusConfigurationDB>();
  }
}
