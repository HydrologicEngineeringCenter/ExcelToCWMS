using Hec.Cwms;
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
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: ExceltoCWMS.exe db.config input.xlsx Excel_sheetname startTime [endTime]");
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




            //Oracle o = Oracle.Connect(dbconfig);

            //CwmsDatabase db = new CwmsDatabase(o);
           // db.SetTimeZone("GMT");

            //db.SetOffice("NWDM");
            //var id = "ABSD.Precip.Inst.15Minutes.0.Raw-LRGS";

            //TimeSeries ts = db.GetTimeSeries(id, DateTime.Now.AddHours(-56), DateTime.Now);
            //ts.WriteToConsole();

            //db.SaveTimeSeries(ts)

            // This method shoudl return for each tsid in sheet return dates values for time window
            //= ReadExcel("myexcel.xlsx", sheetname, startTime, endTime)
                                    // for TimeSeries
            
            ClosedXML c = new ClosedXML(filename);
            DataTable dt = c.GetDataTable(sheetName);
            Console.WriteLine();
            Dictionary<string, string[]> dictString = ProcessDataTable.CreateRateDictionary(dt);
            ProcessDataTable.PrintStringDict(dictString);

            //for each ts in dict create a new TimeSeries an add data
            foreach (KeyValuePair<string, string[]> entry in dictString)
            {
                if (!entry.Key.Equals("Date")){
                    TimeSeries myts = new TimeSeries();
                    for (int j = 0; j<entry.Value.Length; j++)
                    {
                        try
                        {
                            Console.WriteLine("Writing TS Data for "+ entry.Key);
                            myts.Add(DateTime.Parse(dictString["Date"][j]), Double.Parse(entry.Value[j]), 0);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.Read();

                        }
                        
                    }
                    //db.SaveTimeSeries(myts)

                }
                
            }

            Console.Read();
            
        }
       

    }
    
}
