using System.Collections.Generic;
using System.Linq;
using HamBusCommonStd;

namespace CoreHambusCommonLibrary.Services
{
  public class ActiveBusesService
  {


    public List<ActiveBusesModel> ActiveBuses { get; set; } = new List<ActiveBusesModel>();
    public void Add(ActiveBusesModel activeBus)
    {
      var exist = ActiveBuses.Any(x => x.ConnectionId == activeBus.ConnectionId);
      if (exist) return;

      ActiveBuses.Add(activeBus);
    }
    public ActiveBusesModel? FindById(string activeId)
    {
      var activeBusObj = ActiveBuses.Find(item => item.ConnectionId == activeId);
      return activeBusObj;
    }
    public ActiveBusesModel? FindByName(string name)
    {
      var activeBusObj = ActiveBuses.Find(item => item.Name == name);
      return activeBusObj;
    }
    public void Remove(string activeBusId)
    {
      var activeBusObj = ActiveBuses.Find(item => item.ConnectionId == activeBusId);
      if (activeBusObj != null)
        ActiveBuses.Remove(activeBusObj);
    }
    public void UpdateState(RigState state)
    {
      var activeBus = FindByName(state.Name);
      if (activeBus != null)
        activeBus.State = state;

    }
  }
}
