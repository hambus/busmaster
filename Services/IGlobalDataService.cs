using System.Collections.Generic;
using System.Threading.Tasks;
using CoreHambusCommonLibrary.DataLib;
using HamBusCommonStd;

namespace CoreHambusCommonLibrary.Services
{
  public interface IGlobalDataService
  {
    Task InsertBusEntry(BusConfigurationDB conf);
    Task UpdateBusEntry(string? name, BusConfigurationDB? conf);
    Task<BusConfigurationDB?> QueryBusByName(string name);
    Task<List<BusConfigurationDB>> GetBusConfigList();
  }
}
