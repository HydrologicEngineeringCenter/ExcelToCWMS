using Hec.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToCWMS
{
    class ProcessDataTable
    {
        //public TimeSeries[] ReturnTimeSeries( )

        public static Dictionary<string, string[]> CreateRateDictionary(DataTable dt)
        {
            Dictionary<string, string[]> tsdict = new Dictionary<string, string[]>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn dataColumn = dt.Columns[i];
                string header = dataColumn.ColumnName.ToString();

                string[] vs = dt.AsEnumerable().Select(r => r.Field<string>(header)).ToArray();
                double[] vd = new double[vs.Length];


                tsdict.Add(header, vs);

            }
            return tsdict;
        }


        public static void PrintStringDict(Dictionary<string, string[]> dict)
        {
            foreach (var pair in dict)
            {
                Console.WriteLine("{0}:", pair.Key);
                foreach (string s in pair.Value)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine();
            }
        }

        internal static TimeSeries[] getTimeSeries(string filename, string sheetName, DateTime startTime, DateTime backDate)
        {
            ClosedXML c = new ClosedXML(filename);
            DataTable dt = c.GetDataTable(sheetName);
            Console.WriteLine();
            Dictionary<string, string[]> dictString = ProcessDataTable.CreateRateDictionary(dt);
            ProcessDataTable.PrintStringDict(dictString);

            //for each ts in dict create a new TimeSeries an add data
            TimeSeries[] retval = new TimeSeries[dictString.Count - 1];
            int mytscounter = 0;
            foreach (KeyValuePair<string, string[]> entry in dictString)
            {
                if (!entry.Key.Equals("Date"))
                {
                    TimeSeries myts = new TimeSeries();
                    for (int j = 0; j < entry.Value.Length; j++)
                    {
                        try
                        {
                            Console.WriteLine("Writing TS Data for " + entry.Key);
                            myts.Add(DateTime.Parse(dictString["Date"][j]), Double.Parse(entry.Value[j]), 0);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.Read();

                        }
                        
                    }
                    retval[mytscounter] = myts;
                    mytscounter += 1;
                    //db.SaveTimeSeries(myts)

                }

            }
            return retval;
        }
    }
}
        
           
            






