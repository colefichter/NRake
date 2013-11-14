using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NRakeCore;
using NRakeCore.StopWordFilters;

namespace UnitTestProject1
{
    [TestClass]
    public class BasicStopWordFilterTests
    {
        [TestMethod]
        public void IsStopWord()
        {
            //Arrange
            IStopWordFilter filter = new BasicStopWordFilter();

            //Act
            var res = filter.IsStopWord("of");

            //Assert
            Assert.IsTrue(res);
        }
    }
}
