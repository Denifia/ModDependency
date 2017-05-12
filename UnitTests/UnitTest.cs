using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var sortedMods = Program.Sort(new IMod[1] { new Mod("a") });
            Assert.AreEqual(1, sortedMods.Length);
        }
    }
}
