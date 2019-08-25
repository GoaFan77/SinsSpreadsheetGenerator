using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinsSpreadsheetGenerator.EntityClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SinsSpreadsheetGenerator
{
    [TestClass()]
    public class UnitTests
    {
        [TestMethod()]
        public void TestLoadProperty()
        {
            Frigate testFrigate = new Frigate("TestFrigate.entity");
            testFrigate.LoadEntityValue("statCountType", "LightFrigate");
            Assert.IsTrue(testFrigate.StatCountType == "LightFrigate");
        }
    }
}
