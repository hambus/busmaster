using System.Collections.Generic;

namespace CoreHambusCommonLibrary.Services
{
  public class ActiveBusesService 
  {
    public int Id { get; set; }
    public string Name { get; set; } = "Unknown";
    public string? Configuration { get; set; }
    public List<string> Ports { get; set; } = new List<string>();

    public ActiveBusesService()
    {

    }
  }
}
