using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightScheduleChangeDetector.Entities;

public class Route
{
    public int RouteId { get; set; }
    public int OriginCityId { get; set; }
    public int DestinationCityId { get; set; }
    public DateTime DepartureDate { get; set; }

    public ICollection<Flight> Flights { get; set; }
}
