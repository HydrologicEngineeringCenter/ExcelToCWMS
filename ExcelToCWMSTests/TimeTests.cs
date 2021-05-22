using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Hec.Data;
using Hec.Utilities;
using NodaTime;
using NodaTime.Text;
using System.Collections.Generic;

namespace ExcelToCWMSTests
{
  [TestClass]
  public class TimeTests
  {



    /// <summary>
    /// 
    /// </summary>
    [TestMethod]
    public void BasicsDateTimeOffset() {

            //If we use DateTimeOffset, we would need to use it in tandem with somehting that..
            //is timezone aware
        //https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset?view=net-5.0
            //Although a DateTimeOffset value includes an offset,
            //it is not a fully time zone-aware data structure.
            //While an offset from UTC is one characteristic of a time zone,
            //it does not unambiguously identify a time zone.
            //Not only do multiple time zones share the same offset from UTC,
            //but the offset of a single time zone changes if it observes daylight saving time.
            //This means that, as soon as a DateTimeOffset value is disassociated from its time zone,
            //it can no longer be unambiguously linked back to its original time zone.

            // CWMS database takes : America/Pacific  as input.
            //  PDT
            /* Brazil/West
      CAT
      CET
      CNT
      CST
      CST6CDT*/
            // ts = db.Read("CST") // constant offset
            // ts = db.Read("CST6CDT") // daylight savings not constant offset
            // ts.TZ = CST6CDT
            // convert CST 
            // ts.TZ == "CST"
            //   db.Write(ts){
            //    needs to convert to GMT.. (java style)  we know 'CST6CDT'
            //}



        }
        [TestMethod]
    public void BasicsNodaTime()
    {       //This is an example of how of a db.write case might work
            var dtlist = new List<DateTime>();
            //a TimeSeries has some ambiguous DateTimes read from excel, though they represent times in TimeSeries.TZ
            //The TimeSeries.TZ is a string olson time "Etc/GMT+8" read from db config
            string tz = "America/Los_Angeles";
            //string tz = "Etc/GMT+8";
            DateTimeZone zone = DateTimeZoneProviders.Tzdb[tz];
            dtlist.Add(new DateTime(2021, 7, 1, 12, 0, 0));
            dtlist.Add(new DateTime(2021, 7, 2, 12, 0, 0));
            dtlist.Add(new DateTime(2021, 7, 3, 12, 0, 0));
            dtlist.Add(new DateTime(2021, 1, 1, 12, 0, 0));
            dtlist.Add(new DateTime(2021, 1, 2, 12, 0, 0));
            dtlist.Add(new DateTime(2021, 1, 3, 12, 0, 0));
            foreach (DateTime dt in dtlist)
            {
                Console.WriteLine("Ambiguous DateTime = " + dt);

                //We convert to ambiguous noda time
                LocalDateTime ldt = LocalDateTime.FromDateTime(dt);   
                
                //Then we convert to unambiguous noda time in the TimeSeries.TZ timezone
                ZonedDateTime zdt = zone.AtStrictly(ldt);
                var pattern = ZonedDateTimePattern.ExtendedFormatOnlyIso;
                Console.WriteLine("ZonedDateTime = " + pattern.Format(zdt));

                //Then convert to utc java milis since unix
                DateTime utcdt = zdt.ToDateTimeUtc();
                DateTimeOffset dtOffset = utcdt;

                //HERE IS WHERE WE SEND JAVA MILIS TO DATABASE db.write(ts)
                long javamillisC = dtOffset.ToUnixTimeMilliseconds();
                Console.WriteLine("java milis utc = " + javamillisC);

                //Convert back to see if DateTime represents ths UTC value
                DateTime dtbackcheck = DateTimeOffset.FromUnixTimeMilliseconds(javamillisC).DateTime;
                TimeSpan diff = dt - dtbackcheck;
                Console.WriteLine("Difference in hours  = " + diff.Hours);
            }


        }

    [TestMethod]
        public void ToUnixTimeMilisecondTest()
        {
            string id = "ACIA.Flow.Inst.1Hour.0.Best-NWDM";
            TimeSeries ts = new TimeSeries(id, "CFS", "America/Chicago");
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
