using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


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
            var TimeSeriesArray=ProcessDataTable.GetTimeSeriesFromExcel("input.xlsx", "import", DateTime.Parse("2021-06-01"), DateTime.Parse("2021-06-03"), "America/Los_Angeles");
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
            DateTime t1 = DateTime.Parse("2021-11-01 07:00");

            DateTime t2 = DateTime.Parse("2021-11-21 07:00");

            var TimeSeriesArray = ProcessDataTable.GetTimeSeriesFromExcel("input.xlsx", "import", t1, t2, "America/Los_Angeles");
            Console.WriteLine( "Start Time: "+ TimeSeriesArray[0].getTSStartTime());
            Console.WriteLine("End Time: " + TimeSeriesArray[0].getTSSEndTime());
            Assert.AreEqual(t1, TimeSeriesArray[0].getTSStartTime());
            Assert.AreEqual(t2, TimeSeriesArray[0].getTSSEndTime());

        }


    }


}
