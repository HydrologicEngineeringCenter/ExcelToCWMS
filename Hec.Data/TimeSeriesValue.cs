using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hec.Data
{
  struct TimeSeriesValue {

    public TimeSeriesValue(double? value, int quality)
    {
      Value = value;
      Quality = quality;
    }
    public double? Value { get; set; }
    public int Quality { get; set; }

  }
}
