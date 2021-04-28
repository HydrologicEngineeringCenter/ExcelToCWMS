﻿using Hec.Cwms;
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

      TestSave(@"C:\utils\db\hec-3.2.2-MRR.txt");

      if (args.Length != 5)
            {
                Console.WriteLine("Usage: ExceltoCWMS.exe db.config input.xlsx Excel_sheetname Date lookBackDays");
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
            DateTime endTime = DateTime.Parse(args[3]);
            int lookBackDays = int.Parse(args[4]);
            DateTime startTime= endTime.AddDays(-lookBackDays);
            Console.WriteLine(startTime);

      


            //Oracle o = Oracle.Connect(dbconfig);

            //CwmsDatabase db = new CwmsDatabase(o);
            //db.SetTimeZone("GMT");

            //db.SetOffice("NWDM");
            //var id = "ABSD.Precip.Inst.15Minutes.0.Raw-LRGS";

            //TimeSeries ts = db.GetTimeSeries(id, DateTime.Now.AddHours(-56), DateTime.Now);
            //ts.WriteToConsole();

            //db.SaveTimeSeries(ts)

            //ClosedXML throws exception when excel wb is open
            TimeSeries[] tsArrays =ProcessDataTable.GetTimeSeriesFromExcel(filename, sheetName, endTime, startTime);
            foreach (TimeSeries ts in tsArrays)
             {
              

            }
            

            Console.Read();
            
        }

    private static void TestSave(string dbconfig)
    {
      string id = "ABSD.Precip.Inst.15Minutes.0.Raw-LRGS";
      TimeSeries ts = new TimeSeries(id);
      ts.Add(new DateTime(2000, 1, 1), 123, 0);
      ts.Add(new DateTime(2000, 1, 2), 456, 1);

      Oracle o = Oracle.Connect(dbconfig);
      CwmsDatabase db = new CwmsDatabase(o);
     // db.SetTimeZone("GMT");
      db.SetOffice("NWDM");

      var ts2 = db.GetTimeSeries(id, DateTime.Now.AddDays(-2), DateTime.Now);
      ts2.WriteToConsole();

      db.SaveTimeSeries(ts);
    }
  }
    
}
