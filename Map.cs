using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System.IO;

namespace Prologue
{
    class Map
    {
        //In this class a Map can be loaded into the GameWorld

        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public static List<Tiles> Tilelist = new List<Tiles>();

        private static int[] EventZones = new int[] { 1 };

        public Map( PrologueContent prologueContent, SpriteBatch spriteBatch)
        {


            var levelData = LoadLevelData("C:/Users/david/source/repos/Prologue/Prologue/Map.Json");
            int[,] Grid = levelData;

            foreach(int EventID in EventZones)
            {
                EventZone.EventZoneList.Add(new EventZone(EventID));
            }

            Objects.ObjectList.Add(new Objects(7, 7, 1));
            Objects.ObjectList.Add(new Objects(3, 6, 1));
            Objects.ObjectList.Add(new Objects(4, 8, 1));
            Objects.ObjectList.Add(new Objects(3, 10, 1));


            MapWidth = Grid.GetLength(1);
            MapHeight = Grid.GetLength(0);
             
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    Tuple<int, int> Coords = Screen.ScreenCords(x, y); // This will all be replaced once Tiles Have been Json-Fied
                    if (Grid[y, x] != 0)
                    {
                        Tilelist.Add(new Tiles(x, y, Grid[y, x], Coords.Item1, Coords.Item2, prologueContent.Tile1, spriteBatch, prologueContent));
                    }
                }
            }
        }

        public int[,] LoadLevelData(string filename)
        {
            using (var streamReader = new StreamReader(filename))
            {
                var serializer = new JsonSerializer();
                return (int[,])serializer.Deserialize(streamReader, typeof(int[,]));
            }
        }
    }

    class SolidTile
    {
        public Tuple<int,int> Cords { get; }
        public int Chunck { get; }

        public static List<SolidTile> AllSolidTiles = new List<SolidTile>();
        public static List<SolidTile> LoadedSolidTiles = new List<SolidTile>();
        public static List<Objects> LoadedObjects = new List<Objects>();
        public static List<Tiles> LoadedTiles = new List<Tiles>();

        public SolidTile(Tuple<int,int> _Cords, int _Chunck)
        {
            this.Cords = _Cords;
            this.Chunck = _Chunck;
        }

    }
}
