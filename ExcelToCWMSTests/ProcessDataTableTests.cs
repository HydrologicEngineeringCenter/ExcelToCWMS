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
    {
        [TestMethod()]
        public void GetTimeSeriesFromExcelTest()
        {
            var TimeSeriesArray=ProcessDataTable.GetTimeSeriesFromExcel("input.xlsx", "import", DateTime.Parse("2021-03-01"), DateTime.Parse("2021-03-03"));
           foreach (var ts in TimeSeriesArray)
            {
                ts.WriteToConsole();
            }
            Assert.Fail();
        }
        [TestMethod()]
        public void GetTSStartTimeTest()
        {
            var TimeSeriesArray = ProcessDataTable.GetTimeSeriesFromExcel("input.xlsx", "import", DateTime.Parse("2021-03-01"), DateTime.Parse("2021-03-03"));
            Console.WriteLine( "Start Time: "+ TimeSeriesArray[0].getTSStartTime());
            Console.WriteLine("End Time: " + TimeSeriesArray[0].getTSSEndTime());
            Assert.Fail();
        }


    }


}