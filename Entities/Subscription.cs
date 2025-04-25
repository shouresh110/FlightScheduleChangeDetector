using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightScheduleChangeDetector.Entities;

public class Subscription
{
    public int AgencyId { get; set; }
    public int OriginCityId { get; set; }
    public int DestinationCityId { get; set; }
}
