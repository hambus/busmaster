using System.Threading.Tasks;
using CoreHambusCommonLibrary.DataLib;

namespace CoreHambusCommonLibrary.Services
{
  public interface IGlobalDataService
  {
    Task InsertBusEntry(BusConfigurationDB conf);
    Task UpdateBusEntry(string? name, BusConfigurationDB? conf);
    Task<BusConfigurationDB?> QueryBusByName(string name);
  }
}
