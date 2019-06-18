using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prologue
{


    class Chunck
    {
        static public int ChunckSize = 16;
        static public int PlayerChunck { get; private set; }

        public static void Update()
        {
            SetPlayerChunck();

            SolidTile.LoadedSolidTiles.Clear();
            SolidTile.LoadedSolidTiles = SolidTile.AllSolidTiles.FindAll(x => x.Chunck == PlayerChunck || x.Chunck == PlayerChunck + 1 || x.Chunck == PlayerChunck - 1);
            SolidTile.LoadedObjects.Clear();
            SolidTile.LoadedObjects = Objects.ObjectList.FindAll(x => x.chunck == PlayerChunck || x.chunck == PlayerChunck + 1 || x.chunck == PlayerChunck - 1);
            SolidTile.LoadedTiles.Clear();
            SolidTile.LoadedTiles = Map.Tilelist.FindAll(x => x.chunck == PlayerChunck || x.chunck == PlayerChunck + 1 || x.chunck == PlayerChunck - 1);

        }

        static public int CurrentChunck(int gridy)
        {
            int _chunck = (int)Math.Ceiling((double)(gridy / ChunckSize));
            return _chunck;
        }

        public static void SetPlayerChunck()
        {
            Chunck.PlayerChunck = CurrentChunck(Player.GetPlayerTiles(Player.Player1).Item2);
            Console.WriteLine(PlayerChunck);
        }
    }
}
