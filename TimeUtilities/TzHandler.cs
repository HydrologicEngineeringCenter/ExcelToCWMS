using System;

namespace TimeUtilities
{
    public class TzHandler
    {
        public static DateTime  ConvertToLocal(DateTime t, TimeSpan tzOffset)
        {
            DateTime localT = t.Add(tzOffset);
            return localT;          
        }

        public static DateTime ConvertToUTC(DateTime t, TimeSpan tzOffset)
        {
            DateTime utcT = t.Add(-tzOffset);
            return utcT;
        }
        public static TimeSpan CreateOffsetTimeSpan(string utcOffset)
        {
            if (!TimeSpan.TryParse(utcOffset + ":00:00", out TimeSpan offset))
            {
                throw new Exception("Could not convert " + utcOffset + " to  offset");
            }
            return offset;
        }
    }
}
