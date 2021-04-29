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
    public long[] TimesAsJavaMilliSeconds()
    {
        var rval = new List<long>();
        foreach (var item in data)
        {
          rval.Add(ToMillisecondsSinceUnixEpoch(item.Key));
        }
        return rval.ToArray();
    }

    private static DateTime UnixEpoch()
    {
      return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    /// <summary>
    /// https://stackoverflow.com/questions/50485294/pass-integer-array-to-oracle-procedure-by-c-sharp
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    private static long ToMillisecondsSinceUnixEpoch(DateTime dateTime)
    {
      return (long)(dateTime - UnixEpoch()).TotalMilliseconds;
    }

        //SortedList is faster than SortedDictionary if inputing data in sorted order
        //https://stackoverflow.com/questions/1376965/when-to-use-a-sortedlisttkey-tvalue-over-a-sorteddictionarytkey-tvalue
        private SortedList<DateTime, TimeSeriesValue> data = new SortedList<DateTime, TimeSeriesValue>();

        public TimeSeries(string id="")
        {
            this.TSID = id;
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
