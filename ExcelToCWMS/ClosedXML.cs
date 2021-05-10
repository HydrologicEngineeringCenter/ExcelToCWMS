using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToCWMS
{
    public class ClosedXML
    {
        string m_filename;
        public ClosedXML(string filename)
        {
            if (IsFileOpen(filename))
            {
                m_filename = CreateWorkingCopy(filename);
            }
            else { m_filename = filename; }
        }
        /// <summary>
        /// read a worksheet as a DataTable
        /// https://stackoverflow.com/questions/45522976/how-to-read-entire-worksheet-data-in-excel-into-datatable-using-closedxml
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataTable(string sheetName)
        {
            var rval = new DataTable();

            using (var wb = new XLWorkbook(m_filename))
            {
                var ws = wb.Worksheet(sheetName);
                var range = ws.Range(ws.FirstCellUsed(), ws.LastCellUsed());

                var columnCount = range.ColumnCount();
                var rowCount = range.RowCount();
                for (var i = 1; i <= columnCount; i++)
                {
                    var columnName = ws.Cell(1, i);
                    rval.Columns.Add(columnName.Value.ToString());
                }
                foreach (var xlRow in range.Rows().Skip(1))
                {
                    DataRow row = rval.NewRow();
                    for (int i = 0; i < xlRow.CellCount(); i++)
                    {
                        var c = xlRow.Cell(i + 1);
                        if (c.HasFormula && c.CachedValue != null)
                        {
                            row[i] = c.CachedValue.ToString();
                        }
                        else
                        {
                            row[i] = c.Value.ToString();
                        }
                    }
                    rval.Rows.Add(row);
                }
                
                return rval;
            }

        }      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="sheetName"></param>
        public void SaveDataTable(DataTable tbl, string sheetName)
        {

            using (var wb = new XLWorkbook(m_filename))
            {
                var ws = wb.Worksheet(sheetName);
                ws.Clear(XLClearOptions.Contents);
                var row = ws.FirstRow();
                if (row.Cell(1).IsEmpty())
                { // add column headers
                    for (int i = 0; i < tbl.Columns.Count; i++)
                    {
                        var c = ws.Cell(1, i + 1);
                        c.SetValue(tbl.Columns[i].ColumnName);
                    }
                }

                row = row.RowBelow();
                var cell = ws.Cell(row.RowNumber(), 1);
                cell.InsertData(tbl);
                wb.Save();
            }


        }

        public string[] SheetNames
        {
            get
            {
                List<string> names = new List<string>();
                using (var wb = new XLWorkbook(m_filename))
                {
                    foreach (IXLWorksheet worksheet in wb.Worksheets)
                    {
                        names.Add(worksheet.Name);
                    }
                }
                return names.ToArray();
            }
        }

        public bool SheetExist(string sheetName)
        {
            return SheetNames.Contains(sheetName, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns false is the specified file is open
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool IsFileOpen(string filename)
        {
            try
            {
                FileInfo f = new FileInfo(filename);
                using (FileStream stream = f.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Copys the given file to the systems temp location and retursn the string file path
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string CreateWorkingCopy(string filename)
        {
            string exstension = Path.GetExtension(filename);
            string tempFilename = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks.ToString()+ exstension);
            Console.WriteLine("Copying File to "+ tempFilename);
            File.Copy(filename, tempFilename, true);
            return tempFilename;

        }
    }
}