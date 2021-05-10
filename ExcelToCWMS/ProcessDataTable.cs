using Hec.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

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
                ParseHeader(header, out string id, out string units, out string tz);
                tsDict.Add(header, new TimeSeries(id, units));
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
        public static void ParseHeader(String header, out string id, out string units, out string tz)
        {
            string re = @"\s{0,1}(?<id>.*){(units=(?<units>\w+))(,\s*timezone=(?<timezone>\w+))?}\s*";
            id = units = tz = "";
            Match m = Regex.Match(header, re);
            if (!m.Success)
            {
                throw new Exception("Could not Parse Column Header " + header);
            }
            id = m.Groups["id"].Value;
            units = m.Groups["units"].Value;
            tz = m.Groups["timezone"].Value;
        }
    }
}

        
           
            






