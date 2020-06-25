using System.Collections.Generic;
using BusMaster.Model;

namespace BusMaster
{
  public class ActiveBusesModel
  {
    public int? Id { get; set; }
    public string? ConnectionId { get; set; }
    public string? Name { get; set; }
    public BusType? Type { get; set; }
    public BusConfigBase? Configuration { get; set; }
    public List<string>? PortList { get; set; }
  }
}
