using System;
using System.Data;

namespace Hec.Data
{
  public interface ITimeSeries
  {
    void Add(DateTime t, double? value, int quality = 0);
    void WriteToConsole();
    TimeSeriesValue this[DateTime t] { get; }
    string TSID { get; set; }
    string Units { get; set; }
  }
}