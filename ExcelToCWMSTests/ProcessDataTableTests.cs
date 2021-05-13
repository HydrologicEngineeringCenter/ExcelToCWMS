using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExcelToCWMS;
using Hec.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToCWMS.Tests
{
    [TestClass()]
    public class ProcessDataTableTests
    {/// <summary>
    /// Description Here
    /// </summary>
        [TestMethod()]
        public void GetTimeSeriesFromExcelTest()
        {
            var TimeSeriesArray=ProcessDataTable.GetTimeSeriesFromExcel("input.xlsx", "import", DateTime.Parse("2021-06-01"), DateTime.Parse("2021-06-03"), new TimeSpan(0,0,0));
           foreach (var ts in TimeSeriesArray)
            {
                ts.WriteToConsole();
            }
            Assert.AreEqual(2, TimeSeriesArray.Length);
        }
        /// <summary>
        /// Description Here
        /// </summary>
        [TestMethod()]
        public void GetTSStartTimeTest()
        {
            DateTime t1 = DateTime.Parse("2021-03-01");
            DateTime t2 = DateTime.Parse("2021-03-03");

            var TimeSeriesArray = ProcessDataTable.GetTimeSeriesFromExcel("input.xlsx", "import", t1, t2, new TimeSpan(0, 0, 0));
            Console.WriteLine( "Start Time: "+ TimeSeriesArray[0].getTSStartTime());
            Console.WriteLine("End Time: " + TimeSeriesArray[0].getTSSEndTime());
            Assert.AreEqual(t1, TimeSeriesArray[0].getTSStartTime());
            Assert.AreEqual(t2, TimeSeriesArray[0].getTSSEndTime());

        }


    }


}