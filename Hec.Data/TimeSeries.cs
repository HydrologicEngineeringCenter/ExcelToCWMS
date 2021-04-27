using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hec.Data
{
    public class TimeSeries : ITimeSeries
    {
        public string TSID {
            get
            { return _tsid; }
            set 
            { _tsid = value;} 
        }
        private string _tsid;
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
            Console.WriteLine("TSID = "+_tsid);
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
