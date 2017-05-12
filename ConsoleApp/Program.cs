using System;
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

        }

        public static IMod[] Sort(IMod[] mods)
        {
            // Kahn's algorithm (https://en.wikipedia.org/wiki/Topological_sorting)
            //L ← Empty list that will contain the sorted elements
            //S ← Set of all nodes with no incoming edges
            //while S is non-empty do
            //    remove a node n from S
            //    add n to tail of L
            //    for each node m with an edge e from n to m do
            //        remove edge e from the graph
            //        if m has no other incoming edges then
            //            insert m into S
            //if graph has edges then
            //    return error (graph has at least one cycle)
            //else 
            //    return L (a topologically sorted order)

            var unsortedMods = mods.ToList();
            var sortedMods = new List<IMod>();
            var modsWithIssues = new Dictionary<string, string>();

            do
            {
                var mod = unsortedMods[0];
                unsortedMods.RemoveAt(0);
                //foreach (var modId in mod.ModManifest.LoadBefore)
                //{
                //    if (ModIdFoundInList(modId, sortedMods))
                //    {

                //    }
                //}
                sortedMods.Add(mod);
            } while (unsortedMods.Any());
                
            return sortedMods.ToArray();
        }

        public static bool ModIdFoundInList(string modId, List<IMod> mods)
        {
            return mods.Any(x => x.ModManifest.UniqueID.Equals(modId));
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
