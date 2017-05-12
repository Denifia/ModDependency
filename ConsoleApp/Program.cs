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
            var a = new Mod("a");
            var b = new Mod("b", loadBefore: Mods("a"));
            var mods = new List<IMod>() { a, b };

            Console.WriteLine("INPUT");
            Console.WriteLine("-----");
            foreach (var mod in mods)
            {
                Console.WriteLine($"\"{mod.ModManifest.UniqueID}\" \n  LoadsBefore: {string.Join(", ", mod.ModManifest.LoadBefore)} \n  LoadsAfter: {string.Join(", ", mod.ModManifest.LoadAfter)}");
            }

            var sortedMods = Sort(mods);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("OUTPUT");
            Console.WriteLine("------");
            foreach (var mod in sortedMods)
            {
                Console.WriteLine($"Loaded \"{mod.ModManifest.UniqueID}\" \n  LoadsBefore: {string.Join(", ", mod.ModManifest.LoadBefore)} \n  LoadsAfter: {string.Join(", ", mod.ModManifest.LoadAfter)}");
            }
            Console.ReadLine();
        }

        private static List<IMod> UnsortedMods = new List<IMod>();

        public static List<IMod> Sort(List<IMod> mods)
        {
            UnsortedMods = mods;
            var sortedMods = new Stack<IMod>();
            var visitedMods = new bool[mods.Count];

            for (int modIndex = 0; modIndex < mods.Count; modIndex++)
            {
                if (visitedMods[modIndex] == false)
                {
                    TopologicalSort(modIndex, visitedMods, sortedMods);
                }
            }
            
            return sortedMods.Reverse().ToList();
        }

        private static void TopologicalSort(int modIndex, bool[] visitedMods, Stack<IMod> sortedMods)
        {
            visitedMods[modIndex] = true;
            var mod = UnsortedMods[modIndex];
            var modsToLoadFirst = UnsortedMods.Where(x =>
                mod.ModManifest.LoadAfter.Contains(x.ModManifest.UniqueID) ||
                x.ModManifest.LoadBefore.Contains(mod.ModManifest.UniqueID)
            ).ToList();

            foreach (var requiredMod in modsToLoadFirst)
            {
                var index = UnsortedMods.IndexOf(requiredMod);
                if (!visitedMods[index])
                {
                    TopologicalSort(index, visitedMods, sortedMods);
                }
            }

            sortedMods.Push(mod);
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
