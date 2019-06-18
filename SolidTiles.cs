using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prologue
{
    class SolidTile
    {
        public Tuple<int, int> Cords { get; }
        public int Chunck { get; }

        public static List<SolidTile> AllSolidTiles = new List<SolidTile>();
        public static List<SolidTile> LoadedSolidTiles = new List<SolidTile>();
        public static List<Objects> LoadedObjects = new List<Objects>();
        public static List<Tiles> LoadedTiles = new List<Tiles>();

        public SolidTile(Tuple<int, int> _Cords, int _Chunck)
        {
            this.Cords = _Cords;
            this.Chunck = _Chunck;
        }

    }
}
