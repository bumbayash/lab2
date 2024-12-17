using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.test
{
    [TestClass]

    public class GameLoggerTest
    {
        [TestMethod]
        public void getInstanceTest()
        {
            GameLogger firstInstance = GameLogger.getInstance();
            GameLogger secondInstance = GameLogger.getInstance();

            Assert.AreEqual(firstInstance, secondInstance);
        }
    }
}
