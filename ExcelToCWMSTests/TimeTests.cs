using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Hec.Data;
using Hec.Utilities;

namespace ExcelToCWMSTests
{
    [TestClass]
    public class TimeTests
    {

        [TestMethod]
        public void ToUnixTimeMilisecondTest()
        {
            string id = "ACIA.Flow.Inst.1Hour.0.Best-NWDM";
            TimeSeries ts = new TimeSeries(id, "CFS", TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
            ts.Add(new DateTime(2021, 5, 1), 123, 0);
            ts.Add(new DateTime(2021, 5, 2), 456, 0);
            ts.Add(new DateTime(2021, 5, 3), 789, 0);
            ts.Add(new DateTime(2021, 1, 1), 123, 0);
            ts.Add(new DateTime(2021, 1, 2), 456, 0);
            ts.Add(new DateTime(2021, 1, 3), 789, 0);
            ts.WriteToConsole();
            long[] milis = ts.ToUnixMillisUTC();
            foreach (long i in milis)
            {
                var dto = DateTimeOffset.FromUnixTimeMilliseconds(i).UtcDateTime;
                Console.WriteLine(dto);
            }
        }
        [TestMethod]
        public void OlsonConverterTest()
        {
           string[] zones = { "America/Los_Angeles", "America/Denver", "America/Chicago", "America/New_York" };
            foreach (string zone in zones)
            {
                TimeZoneInfo rval = TimeUtilities.OlsonTimeZoneToTimeZoneInfo(zone);
                DateTime dt = new DateTime(2021, 5, 1, 0,0,0, DateTimeKind.Local);            
                Console.WriteLine(zone + "   was converted to   " + rval.Id);
                Console.WriteLine("Base offset is: " + rval.BaseUtcOffset);
                Console.WriteLine("Current offset is: " + rval.GetUtcOffset(dt));
                

            }


        }
    }
}
