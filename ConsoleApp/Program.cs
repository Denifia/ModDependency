using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Success
            var a = new Mod("a", loadBefore: Mods("c"));
            var b = new Mod("b", loadBefore: Mods("a"));
            var c = new Mod("c", loadAfter: Mods("e"));
            var d = new Mod("d", loadAfter: Mods("b"), loadBefore: Mods("e"));
            var e = new Mod("e", loadAfter: Mods("a"));
            var f = new Mod("f");
            var g = new Mod("g");
            var mods = new List<IMod>() { a, b, c, d, e, f, g };

            // Circular
            //var a = new Mod("a", loadBefore: Mods("b"));
            //var b = new Mod("b", loadBefore: Mods("a"));
            //var mods = new List<IMod>() { a, b };

            Console.WriteLine("INPUT");
            Console.WriteLine("-----");
            foreach (var mod in mods)
            {
                Console.WriteLine($"\"{mod.ModManifest.UniqueID}\" ( LoadsBefore: {string.Join(", ", mod.ModManifest.LoadBefore)} | LoadsAfter: {string.Join(", ", mod.ModManifest.LoadAfter)} )");
            }

            Console.WriteLine();
            Console.WriteLine();
            var sortedMods = Sort(mods);

            if (sortedMods.Any())
            {
                Console.WriteLine("OUTPUT");
                Console.WriteLine("------");
                foreach (var mod in sortedMods)
                {
                    Console.WriteLine($"Loaded \"{mod.ModManifest.UniqueID}\" ( LoadsBefore: {string.Join(", ", mod.ModManifest.LoadBefore)} | LoadsAfter: {string.Join(", ", mod.ModManifest.LoadAfter)} )");
                }
            }
            Console.ReadLine();
        }

        private static List<IMod> UnsortedMods = new List<IMod>();

        public static List<IMod> Sort(List<IMod> mods)
        {
            UnsortedMods = mods;
            var sortedMods = new Stack<IMod>();
            var visitedMods = new bool[mods.Count];

            var currentChain = new List<IMod>();
            var success = true;
            for (int modIndex = 0; modIndex < mods.Count; modIndex++)
            {
                if (visitedMods[modIndex] == false)
                {
                    success = TopologicalSort(modIndex, visitedMods, sortedMods, currentChain);
                }
                if (!success) break;
            }
            
            if (!success)
            {
                // Failed to sort list, return no mods.
                return new List<IMod>();
            }

            return sortedMods.Reverse().ToList();
        }

        private static bool TopologicalSort(int modIndex, bool[] visitedMods, Stack<IMod> sortedMods, List<IMod> currentChain)
        {
            visitedMods[modIndex] = true;
            var mod = UnsortedMods[modIndex];
            var modsToLoadFirst = UnsortedMods.Where(x =>
                mod.ModManifest.LoadAfter.Contains(x.ModManifest.UniqueID) ||
                x.ModManifest.LoadBefore.Contains(mod.ModManifest.UniqueID)
            ).ToList();

            // TODO: Add code to check for missing mods

            var circularMods = currentChain.FirstOrDefault(x => modsToLoadFirst.Contains(x));
            if (circularMods != null)
            {
                Console.WriteLine($"Circular reference found.");
                var chain = $"{mod.ModManifest.UniqueID} -> {circularMods.ModManifest.UniqueID}";
                for (int i = currentChain.Count - 1; i >= 0; i--)
                {
                    chain = $"{currentChain[i].ModManifest.UniqueID} -> " + chain;
                    if (currentChain[i].ModManifest.UniqueID.Equals(mod.ModManifest.UniqueID)) break;
                }
                Console.WriteLine(chain);
                return false;
            }

            currentChain.Add(mod);

            var success = true;
            foreach (var requiredMod in modsToLoadFirst)
            {
                var index = UnsortedMods.IndexOf(requiredMod);
                if (!visitedMods[index])
                {
                    success = TopologicalSort(index, visitedMods, sortedMods, currentChain);
                }
                if (!success) break;
            }

            sortedMods.Push(mod);
            currentChain.Remove(mod);
            return success;
        }

        public static string[] Mods(params string[] mods)
        {
            return mods;
        }
    }

    public interface IMod
    {
        IManifest ModManifest { get; }
        void Entry();
    }

    public class Mod : IMod
    {
        private IManifest _manifest;
        public IManifest ModManifest => _manifest;

        public Mod(string id)
        {
            _manifest = new Manifest(id);
        }

        public Mod(string id, string[] loadBefore = null, string[] loadAfter = null)
        {
            _manifest = new Manifest(id)
            {
                LoadBefore = loadBefore ?? null,
                LoadAfter = loadAfter ?? null
            };
        }

        public void Entry()
        {
            Console.WriteLine($"Called entry for {_manifest.UniqueID}");
        }
    }

    public interface IManifest
    {
        string UniqueID { get; set; }
        string[] LoadBefore { get; set; }
        string[] LoadAfter { get; set; }
    }

    public class Manifest : IManifest
    {
        private string _uniqueID;
        public string UniqueID {
            get => _uniqueID;
            set => _uniqueID = value;
        }

        private string[] _loadBefore = new string[0];
        public string[] LoadBefore {
            get => _loadBefore;
            set
            {
                if (value == null)
                {
                    _loadBefore = new string[0];
                }
                else
                {
                    _loadBefore = value;
                }
            }
        }

        private string[] _loadAfter = new string[0];
        public string[] LoadAfter
        { 
            get => _loadAfter;
            set
            {
                if (value == null)
                {
                    _loadAfter = new string[0];
                }
                else
                {
                    _loadAfter = value;
                }
            }
        }

        public Manifest(string id)
        {
            _uniqueID = id;
        }
    }    
}
