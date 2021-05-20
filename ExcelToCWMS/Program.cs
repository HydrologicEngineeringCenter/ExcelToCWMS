using Hec.Cwms;
using Hec.Data;
using System;
using Hec.Utilities;

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
                Console.WriteLine("Usage: 'ExceltoCWMS.exe db.config input.xlsx Excel_sheetname Date lookBackDays'");
                Console.WriteLine();
                Console.WriteLine("About: ExcelToCWMS reads time series data from a formatted excel sheet and writes to the Oracle Database");
                Console.WriteLine();
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
            DateTime startTime = endTime.AddDays(-lookBackDays);
            
            var cr = new ConfigReader(dbconfig);

            Console.WriteLine("UTC  Offset from config = "+ cr.CRead("timezone"));
            TimeZoneInfo tzInfo = TimeUtilities.OlsonTimeZoneToTimeZoneInfo(cr.CRead("timezone"));

            Oracle o = Oracle.Connect(cr.CRead("user"), cr.CRead("host"), cr.CRead("sid"), cr.CRead("port"));
            CwmsDatabase db = new CwmsDatabase(o, cr.CRead("officeid"));

            TimeSeries[] tsArrays = ProcessDataTable.GetTimeSeriesFromExcel(filename, sheetName, startTime, endTime ,tzInfo);
            foreach (TimeSeries ts in tsArrays)
            {
                db.WriteTimeSeries(ts);
                db.ReadTimeSeries(ts.TSID, startTime, endTime, cr.CRead("timezone")).WriteToConsole();
            }
            Console.Read();
        }


        private static void TestSave(string dbconfig)
        {
            string id = "ABSD.Precip.Inst.15Minutes.0.Raw-LRGS";
            TimeSeries ts = new TimeSeries(id, "mm", TimeUtilities.OlsonTimeZoneToTimeZoneInfo("America/Chicago"));
            ts.Add(new DateTime(2000, 1, 1), 123, 0);
            ts.Add(new DateTime(2000, 1, 2), 456, 1);
            var cr = new ConfigReader(dbconfig);
            Oracle o = Oracle.Connect(cr.CRead("user"), cr.CRead("host"), cr.CRead("sid"), cr.CRead("port"));
            CwmsDatabase db = new CwmsDatabase(o, cr.CRead("officeid"));

            db.WriteTimeSeries(ts);

            var ts2 = db.ReadTimeSeries(id, DateTime.Now.AddDays(-2), DateTime.Now,cr.CRead("timezone"));
            ts2.WriteToConsole();
        }

        private static void TestPrint(CwmsDatabase db)
        {
            var id = "ACIA.Flow.Inst.1Hour.0.Best-NWDM";
            ///var id = "ABSD.Precip.Inst.15Minutes.0.Raw-LRGS";
            TimeSeries ts = db.ReadTimeSeries(id, DateTime.Now.AddHours(-56), DateTime.Now, "America/Los_Angeles");
            ts.WriteToConsole();
            TimeSeries ts2 = db.ReadTimeSeries(id, DateTime.Now.AddHours(-56), DateTime.Now, "UTC");
            ts2.WriteToConsole();

        }
    }
}
