using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void BasicCountTest()
        {
            var sortedMods = Program.Sort(new IMod[1] { new Mod("a") });
            Assert.AreEqual(1, sortedMods.Length);
        }

        [TestMethod]
        public void SimpleBeforeTest()
        {
            var a = new Mod("a");
            var b = new Mod("b", loadBefore: Mods("a"));
            var mods = new List<IMod>() { a, b };

            var sortedMods = Program.Sort(mods.ToArray()).ToList();

            Assert.IsTrue(sortedMods.IndexOf(b) < sortedMods.IndexOf(a), "Mod b was not loaded before mod a.");
        }

        public string[] Mods(params string[] mods)
        {
            return mods;
        }
    }
}
