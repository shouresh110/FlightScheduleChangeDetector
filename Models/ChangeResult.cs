using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightScheduleChangeDetector.Models;

public class ChangeResult
{
    public int FlightId { get; set; }
    public int OriginCityId { get; set; }
    public int DestinationCityId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int AirlineId { get; set; }
    public string Status { get; set; } // "New" or "Discontinued"
}
