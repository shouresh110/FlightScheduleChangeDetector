using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightScheduleChangeDetector.Entities;

public class Flight
{
    public int FlightId { get; set; }
    public int RouteId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int AirlineId { get; set; }

    public Route Route { get; set; }
}
