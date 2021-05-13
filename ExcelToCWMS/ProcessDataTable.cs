using Hec.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using TimeUtilities;

namespace ExcelToCWMS
{
    public class ProcessDataTable
    {
        public static TimeSeries[] GetTimeSeriesFromExcel(string filename, string sheetName, DateTime startTime, DateTime endTime, TimeZoneInfo tzInfo)
        {
            ClosedXML c = new ClosedXML(filename);
            DataTable dt = c.GetDataTable(sheetName);
            //This Dict is <column header of DataTable, TImeSeries>
            Dictionary<string, TimeSeries> tsDict = new Dictionary<string, TimeSeries>();
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.ToLower() == "date") continue;
                string header = dc.ColumnName;
                ParseHeader(header, out string id, out string units);
                tsDict.Add(header, new TimeSeries(id, units, tzInfo));
            }
            //https://www.c-sharpcorner.com/blogs/filter-datetime-from-datatable-in-c-sharp1

            foreach (DataRow row in dt.Rows)
            {
                DateTime t = ParseExcelDate(row[0].ToString());

                if (!(t >= startTime && t <= endTime))
                {
                    Console.WriteLine("Time " + t + " outside of specified range");
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
        public static void ParseHeader(String header, out string id, out string units)
        {
            string re = @"\s{0,1}(?<id>.*){(units=(?<units>\w+))(,\s*timezone=(?<timezone>\w+))?}\s*";
            id = units = "";
            Match m = Regex.Match(header, re);
            if (!m.Success)
            {
                throw new Exception("Could not Parse Column Header " + header);
            }
            id = m.Groups["id"].Value;
            units = m.Groups["units"].Value;
        }
        /// <summary>
        /// Sometimes the date from Excel is a string, other times it is an OA Date:
        /// Excel stores date values as a Double representing the number of days from January 1, 1900.
        /// Need to use the FromOADate method which takes a Double and converts to a Date.
        /// OA = OLE Automation compatible.
        /// </summary>
        /// <param name="date">a string to parse into a date</param>
        /// <returns>a DateTime value; if the string could not be parsed, returns DateTime.MinValue</returns>
        public static DateTime ParseExcelDate(string date)
        {
            DateTime dt;
            if (DateTime.TryParse(date, out dt))
            {
                return dt;
            }

            double oaDate;
            if (double.TryParse(date, out oaDate))
            {
                return DateTime.FromOADate(oaDate);
            }
            throw new Exception("Could not convert date " + date + "to DateTime ");
        }
    }
}

        
           
            






