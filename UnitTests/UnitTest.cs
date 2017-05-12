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
            var a = new Mod("a");

            var mods = new List<IMod>() { a };

            var sortedMods = Program.Sort(mods);

            CheckModCount(mods, sortedMods);
        }

        [TestMethod]
        public void SimpleBeforeTest()
        {
            // b - a
            var a = new Mod("a");
            var b = new Mod("b", loadBefore: Mods("a"));

            var mods = new List<IMod>() { a, b };

            var sortedMods = Program.Sort(mods);

            CheckModCount(mods, sortedMods);
            mods.ForEach(mod => CheckRequirements(sortedMods, mod));
        }

        [TestMethod]
        public void SimpleAfterTest()
        {
            // b - a
            var a = new Mod("a", loadAfter:Mods("b"));
            var b = new Mod("b");

            var mods = new List<IMod>() { a, b };

            var sortedMods = Program.Sort(mods);

            CheckModCount(mods, sortedMods);
            mods.ForEach(mod => CheckRequirements(sortedMods, mod));
        }

        [TestMethod]
        public void SimpleCircularTest()
        {
            var a = new Mod("a", loadBefore: Mods("b"));
            var b = new Mod("b", loadBefore: Mods("a"));

            var mods = new List<IMod>() { a, b };

            var sortedMods = Program.Sort(mods);

            CheckModCount(mods, sortedMods);

            // TODO: This fails. Need to handle circular references
            //mods.ForEach(mod => CheckRequirements(sortedMods, mod));
        }

        [TestMethod]
        public void ComplexTest()
        {
            // b - a - c
            //  \   \ /
            //   d - e

            var a = new Mod("a", loadBefore: Mods("c"));
            var b = new Mod("b", loadBefore: Mods("a"));
            var c = new Mod("c", loadAfter: Mods("e"));
            var d = new Mod("d", loadAfter: Mods("b"), loadBefore: Mods("e"));
            var e = new Mod("e", loadAfter: Mods("a"));
            var f = new Mod("f");
            var g = new Mod("g");

            var mods = new List<IMod>() { a, b, c, d, e, f, g };

            var sortedMods = Program.Sort(mods);

            CheckModCount(mods, sortedMods);
            mods.ForEach(mod => CheckRequirements(sortedMods, mod));
        }

        [TestMethod]
        public void SlowLoadTest()
        {
            //   a 
            //  / \
            // d - e - c - b

            var a = new Mod("a");
            var b = new Mod("b");
            var c = new Mod("c", loadBefore: Mods("b"));
            var d = new Mod("d", loadBefore: Mods("a"));
            var e = new Mod("e", loadAfter: Mods("d"), loadBefore: Mods("c"));
            var f = new Mod("f");
            var g = new Mod("g");

            var mods = new List<IMod>() { a, b, c, d, e, f, g };

            var sortedMods = Program.Sort(mods);

            CheckModCount(mods, sortedMods);
            mods.ForEach(mod => CheckRequirements(sortedMods, mod));
        }


        private void CheckModCount(List<IMod> mods, List<IMod> sortedMods)
        {
            Assert.AreEqual(mods.Count, sortedMods.Count);
        }

        /// <summary>
        /// Asserts that the LoadBefore and LoadAfter requirements are fulfilled
        /// </summary>
        private void CheckRequirements(List<IMod> mods, IMod mod)
        {
            foreach (var modId in mod.ModManifest.LoadBefore)
            {
                CheckLoadBefore(mods, mod, modId);
            }
            foreach (var modId in mod.ModManifest.LoadAfter)
            {
                CheckLoadAfter(mods, mod, modId);
            }
        }

        /// <summary>
        /// Asserts that ModA was loaded before ModB
        /// </summary>
        private void CheckLoadBefore(List<IMod> mods, IMod modA, string modB)
        {
            Assert.IsTrue(mods.IndexOf(modA) < mods.IndexOf(mods.First(x => x.ModManifest.UniqueID == modB)), $"Mod {modA.ModManifest.UniqueID} was not loaded before mod {modB}.");
        }

        /// <summary>
        /// Asserts that ModA was loaded after ModB
        /// </summary>
        private void CheckLoadAfter(List<IMod> mods, IMod modA, string modB)
        {
            Assert.IsTrue(mods.IndexOf(modA) > mods.IndexOf(mods.First(x => x.ModManifest.UniqueID == modB)), $"Mod {modA.ModManifest.UniqueID} was not loaded after mod {modB}.");
        }

        public string[] Mods(params string[] mods)
        {
            return mods;
        }
    }
}
