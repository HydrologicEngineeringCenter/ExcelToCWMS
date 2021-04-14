using Hec.Cwms;
using Hec.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToCWMS
{
  class Program
  {
    static void Main(string[] args)
    {
            //----------------------------
            //inputs:
            if (args.Length != 5)
            {
                Console.WriteLine("Usage: ExceltoCWMS.exe db.config input.xlsx Excel_sheetname startTime lookBackDays");
                Console.WriteLine();
                Console.WriteLine("Read time series data from formatted excel sheet and stuff in Oracle");
                Console.WriteLine();
                Console.WriteLine("Example: ExceltoCWMS.exe....");
                Console.WriteLine("");
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine("arg[" + i + "] : " + args[i]);
                }
                return;
            }
            String dbconfig = args[0];
            String filename = args[1];
            String sheetName = args[2];
            DateTime startTime = DateTime.Parse(args[3]);
            int lookBackDays = int.Parse(args[4]);
            DateTime backDate= startTime.AddDays(-lookBackDays);
            Console.WriteLine(backDate);




            //Oracle o = Oracle.Connect(dbconfig);

            //CwmsDatabase db = new CwmsDatabase(o);
            // db.SetTimeZone("GMT");

            //db.SetOffice("NWDM");
            //var id = "ABSD.Precip.Inst.15Minutes.0.Raw-LRGS";

            //TimeSeries ts = db.GetTimeSeries(id, DateTime.Now.AddHours(-56), DateTime.Now);
            //ts.WriteToConsole();

            //db.SaveTimeSeries(ts)

            //ClosedXML throws exception when excel wb is open
            TimeSeries[] tsArrays =ProcessDataTable.getTimeSeries(filename, sheetName, startTime, backDate);
            

            Console.Read();
            
        }
       

    }
    
}
