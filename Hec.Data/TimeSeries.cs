using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hec.Data
{
    public class TimeSeries
    {
    public string TSID { get; set; }

    public string Units { get; set; }

    public TimeZoneInfo TimeZone { get; }


    public double[] Values
    {
      get
      {
        var rval = new List<double>();
        foreach (var item in data)
        {
          rval.Add(item.Value.Value.Value);
        }
        return rval.ToArray();
      }
    }
    public int[] Qualities
    {
      get
      {
        var rval = new List<int>();
        foreach (var item in data)
        {
          rval.Add(item.Value.Quality);
        }
        return rval.ToArray();
      }
    }
        /// <summary>
        /// Converts each DateTime of TimeSeries to miliseconds since unix epoch
        ///Note that the ToUnixTimeMiliseconds() method converts the current instance to UTC before returning the number of milliseconds
        /// https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimemilliseconds?view=net-5.0https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.tounixtimemilliseconds?view=net-5.0
        /// </summary>
        /// <returns>array of longs representing miliseconds since the unix epoch</returns>
        public long[] ToUnixMillisUTC()
        {
            var rval = new List<long>();
            foreach (var item in data)
            {
                DateTimeOffset dtOffset = item.Key;
                long javamillisC = dtOffset.ToUnixTimeMilliseconds();
                rval.Add(javamillisC);
            }
            return rval.ToArray();

        }

        //SortedList is faster than SortedDictionary if inputing data in sorted order
        //https://stackoverflow.com/questions/1376965/when-to-use-a-sortedlisttkey-tvalue-over-a-sorteddictionarytkey-tvalue
        private SortedList<DateTime, TimeSeriesValue> data = new SortedList<DateTime, TimeSeriesValue>();

        public TimeSeries(string id = "", string units = "")
        {
            this.TSID = id;
            this.Units = units;
            this.TimeZone = TimeZoneInfo.Utc;
        } public TimeSeries(string id , string units, TimeZoneInfo  tzInfo)
        {
            this.TSID = id;
            this.Units = units;
            this.TimeZone = tzInfo;
        }

        public TimeSeriesValue this[DateTime t]
        {
            get
            {
                return data[t];
            }
        }

        public void Add(DateTime t, double? value, int quality = 0)
        {
            data.Add(t, new TimeSeriesValue(value, quality));
        }

        public void WriteToConsole()
        {
            Console.WriteLine("TSID = "+TSID);
            Console.WriteLine("Units = " + Units);
            Console.WriteLine("Timezone = " + TimeZone.DisplayName);
            foreach (var item in data)
            {
                Console.WriteLine("{0:dd-MMM-yyyy HHmm}{1,10:f3}{2,8:d}", item.Key, item.Value.Value, item.Value.Quality);
            }
        }
        public DateTime getTSStartTime()
        {
            return data.Keys.FirstOrDefault();
        }
        public DateTime getTSSEndTime()
        {
            return data.Keys.LastOrDefault();
        }
   
  }
}
