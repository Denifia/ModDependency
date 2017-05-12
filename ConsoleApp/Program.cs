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
        public string[] LoadBefore { get; set; }
        public string[] LoadAfter { get; set; }

        public Manifest(string id)
        {
            _uniqueID = id;
        }
    }    
}
