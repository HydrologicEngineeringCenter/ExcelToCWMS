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
    }
}
        
           
            






