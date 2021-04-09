using Hec.Cwms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToCWMS
{
  class Program
  {
    static void Main(string[] args)
    {
      Oracle o = Oracle.Connect(args[0]);

      CwmsDatabase db = new CwmsDatabase(o);
      //db.SetTimeZone("GMT")

      db.SetOffice("NWDM");
      var id = "ABSD.Precip.Inst.15Minutes.0.Raw-LRGS";

      TimeSeries ts = db.GetTimeSeries(id,DateTime.Now.AddHours(-56),DateTime.Now);
      ts.WriteToConsole();
      // Read TS from excel.
      //MVN_ExcelReader er = new MVN_ExcelReader(); // details about their 
      // 
      // db.SaveTimeSeries(ts)



    }
  }
}
