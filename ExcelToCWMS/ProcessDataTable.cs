using Hec.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExcelToCWMS
{
    public class ProcessDataTable
    {
        public static TimeSeries[] GetTimeSeriesFromExcel(string filename, string sheetName, DateTime startTime, DateTime endTime)
        {
            ClosedXML c = new ClosedXML(filename);
            DataTable dt = c.GetDataTable(sheetName);
            //This Dict is <column header of DataTable, TImeSeries>
            Dictionary<string, TimeSeries> tsDict = new Dictionary<string, TimeSeries>();
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.ToLower() == "date") continue;
                string header = dc.ColumnName;
                var parts = ParseHeader(header);
                tsDict.Add(header, new TimeSeries(parts[0], parts[1]));
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

        private static List<string> ParseHeader(string header)
        {
            if (!header.Contains("{units="))
            {
                throw new Exception("COULD NOT FIND UNITS  PARAMETER IN '" + header + "'\n" +
                    "Use Convetion '02600.Flow.Inst.~1Day.0.DailyComputed{units=CFS}'");
            }
            string[] headerparts = header.Split('{', '}');
            if (headerparts.Length > 3)
            {
                throw new Exception("PROBLEM Parsing'" + header + "'\n" +
                    "Use Convetion '02600.Flow.Inst.~1Day.0.DailyComputed{units=CFS}'");
            }
            List<string> partslist = new List<string>();
            partslist.Add(headerparts[0]);
            partslist.Add(headerparts[1].Split('=')[1]);
            return partslist;
        }

        //playing with regex
        public static bool useRegex(String header)
        {
            Regex regex = new Regex("^.*\\{.*=.*\\}$", RegexOptions.IgnoreCase);
            return regex.IsMatch(header);

        }
    }
}

        
           
            






