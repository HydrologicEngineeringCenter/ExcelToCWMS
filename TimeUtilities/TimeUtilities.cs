using NodaTime;
using NodaTime.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hec.Utilities
{
    public class TimeUtilities
    {
        public static long DateTimeAndZoneToUnixMilis(DateTime dt, string IANA_timezone)
        {
            //Create a noda DateTImeZone
            DateTimeZone zone = DateTimeZoneProviders.Tzdb[IANA_timezone];
            //Convert to ambiguous DateTime to ambiguous noda time
            LocalDateTime ldt = LocalDateTime.FromDateTime(dt);

            //Convert to unambiguous noda time in the TimeSeries.TZ timezone
            ZonedDateTime zdt = zone.AtStrictly(ldt);

            //Conver To UTC DateTime 
            DateTime utcdt = zdt.ToDateTimeUtc();
            DateTimeOffset dtOffset = utcdt;

            //Then convert to utc java milis since unix
            return dtOffset.ToUnixTimeMilliseconds();

        }
    }
}
