using Hec.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToCWMS
{
    public class ProcessDataTable
    {     
           public static TimeSeries[] GetTimeSeriesFromExcel(string filename, string sheetName, DateTime startTime, DateTime endTime)
        {
            ClosedXML c = new ClosedXML(filename);
            DataTable dt = c.GetDataTable(sheetName);
            //List <String> tsids = new List<String>();
            Dictionary<string, TimeSeries> tsDict = new Dictionary<string, TimeSeries>();
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.ToLower() == "date") continue;
                string header = dc.ColumnName;
                string tsid = header.Split('{','}')[0];
                string units = header.Split('{', '}')[1].Split('=')[1];
                tsDict.Add(header, new TimeSeries(tsid,units));

            }
            //https://www.c-sharpcorner.com/blogs/filter-datetime-from-datatable-in-c-sharp1
            
            foreach (DataRow row in dt.Rows)
            {
                if (!DateTime.TryParse(row[0].ToString(), out DateTime t))
                {
                    throw new Exception("Could not convert date " + row[0].ToString() + "to DateTime ");
                }
                if (!(t >= startTime && t <= endTime))
                {
                    continue;
                }              
                foreach (string header in tsDict.Keys)
                {
                    string value = row.Field<string>(header);
                    if (!double.TryParse(value, out double dval))
                    {
                        throw new Exception("Could not convert " + value + "to double ");
                    }
                    tsDict[header].Add(t, dval);
                }               
            }                  
            return tsDict.Values.ToArray();

        }
    }
}
        
           
            






