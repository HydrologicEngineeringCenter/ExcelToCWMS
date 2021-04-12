using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExcelToCWMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ExcelToCWMS.Tests
{
    [TestClass()]
    public class ReadExcelTests
    {
        ReadExcel excel = new ReadExcel();
        [TestMethod()]
        public void ReadExcelFileTest()
        {  
            DataTable dt = excel.OLEDBReadExcelFile("FLOW", "C:/Users/HEC/source/repos/ExcelToCWMS/input.xlsx");
            Console.WriteLine(dt);
            Assert.Fail();
        }
        [TestMethod()]
        public void ReadExcelTest2()
        {
            excel.getExcelFile();

        }
    }

}