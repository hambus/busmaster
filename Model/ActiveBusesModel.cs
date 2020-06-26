﻿using System.Collections.Generic;
using BusMaster.Model;

namespace BusMaster
{
  public class ActiveBusesModel
  {
    public int? Id { get; set; }
    public string? Name { get; set; }
    public BusConfigBase? Configuration { get; set; }
    public List<string> Groups { get; set; } = new List<string>();
    public List<string> Ports { get; set; } = new List<string>();

    public BusType? Type { get; set; }

  }
}
