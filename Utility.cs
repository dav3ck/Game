using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Prologue
{
    //General screen information/ functions used by every Class

    public static class Screen
    {
        static public float ScreenWidth { get; set; }
        static public float ScreenHeight { get; set; }

        static public int MinGridX = 16;
        static public int MinGridY = 9;

        static public int ChunckSize = 16;

        static public float GridSize { get; set; }

        static public float CameraX { get; set; }
        static public float CameraY { get; set; }

        static public Tuple<int, int> ScreenCords(int x, int y)
        {
            //Console.WriteLine(GridSize);
            //Console.WriteLine(x * GridSize + " " + y * GridSize);
            return Tuple.Create((int)(x * GridSize),(int)(y * GridSize));
        }

        static public int CurrentChunck(int gridy)
        {
            int _chunck = (int)Math.Ceiling((double)(gridy / ChunckSize)); 
            return _chunck;
        }

        static public Tuple<int, int> GridCords(float x, float y)
        {
            //Console.WriteLine( x+ " " + y);
            return Tuple.Create((int)Math.Floor(x / GridSize), (int)Math.Floor(y / GridSize));
        }

        static public void CameraMovement(float PlayerX, float PlayerY)
        {
            CameraX = PlayerX - (ScreenWidth / 2);
            CameraY = PlayerY - (ScreenHeight / 2);

            //Console.WriteLine(CameraX + " " + PlayerX + "   ---   " + CameraY + " " + PlayerY);
        }

    }

    public static class Utility
    {
        public static List<Tuple<int, int>> GeneratePath(Tuple<int, int> StartPosition, Tuple<int, int> Goal)
        {
            PathFinderTile.PathTiles.Clear();

            List<Tuple<int, int>> Path = new List<Tuple<int, int>>();



          /*  var SolidTiles = new List<Tiles>(Map.Tilelist.FindAll(x => x.Solid == true && x.Y.IsBetween(StartPosition.Item2, Goal.Item2)));
            var SolidObjects = new List<Objects>(Objects.ObjectList.FindAll(x => x.Solid == true && x.Ytile.IsBetween(StartPosition.Item2, Goal.Item2))); */

            /*foreach(var x in )
            {
                //Console.WriteLine("The X Cord: " + x.X + " -- The Y Cord: " + x.Y );
                //PathFinderTile.PathTiles.Add(new PathFinderTile(Tuple.Create(x.X, x.Y)));
            }*/

            //Console.WriteLine("StartPositions: " + StartPosition + " -- " + Goal);

            for (int x = 0; x <= Math.Abs(StartPosition.Item1 - Goal.Item1); x++)
            {
                int _TempXCord;

                if (StartPosition.Item1 >= Goal.Item1)
                {
                    _TempXCord = StartPosition.Item1 - x;
                }
                else
                {
                    _TempXCord = StartPosition.Item1 + x;
                }

                for (int y = 0; y <= Math.Abs(StartPosition.Item2 - Goal.Item2); y++)
                {

                    int _TempYCord;
                    if (StartPosition.Item2 >= Goal.Item2)
                    {
                        _TempYCord = StartPosition.Item2 - y;
                    }
                    else
                    {
                        _TempYCord = StartPosition.Item2 + y;
                    }


                    PathFinderTile.PathTiles.Add(new PathFinderTile(Tuple.Create(_TempXCord, _TempYCord)));
                    //Console.WriteLine("ZIT TUSSEN GOALS :" + Tuple.Create(_TempXCord, _TempYCord));
                }
            }


            PathFinderTile Tile = PathFinderTile.PathTiles.Find(x => x.Cords.Equals(StartPosition));
            Tile.On = true;
            Tile.Beginnin = true;


            List<PathFinderTile> DeletedTiles = new List<PathFinderTile>();
            DeletedTiles.Clear();

            foreach (SolidTile _Temp in SolidTile.AllSolidTiles)
            {
                //PathFinderTile SolidTile = PathFinderTile.PathTiles.Find(x => x.Cords.Item1 == _Temp.X && x.Cords.Item2 == _Temp.Y && _Temp.Solid == true && ( x.Beginnin != true || x.Cords.Equals(Goal)));
                PathFinderTile SolidTile = PathFinderTile.PathTiles.Find(x => x.Cords.Equals(_Temp.Cords) && (x.Beginnin != true || x.Cords.Equals(Goal)));
                DeletedTiles.Add(SolidTile);
            }

            /*foreach (Objects _Temp in SolidObjects)
            {
                PathFinderTile SolidTile = PathFinderTile.PathTiles.Find(x => x.Cords.Item1 == _Temp.Xtile && x.Cords.Item2 == _Temp.Ytile && _Temp.Solid == true && (x.Beginnin != true || x.Cords.Equals(Goal)));
                DeletedTiles.Add(SolidTile);
            }*/



            foreach (PathFinderTile z in DeletedTiles)
            {
                PathFinderTile.PathTiles.Remove(z);
            }


            int Itteration = 0;

            while (!PathFinderTile.PathTiles.Any(x => x.Cords.Equals(Goal) && x.On == true))
            {
                if (Itteration > 1)
                {
                    Console.WriteLine("CROSSED MAX ITTERATIONS --------------");
                    return ImpossiblePath(StartPosition, Goal);
                }
                PathFinderTile.CheckForPath();
                Itteration++;
            }


            PathFinderTile PathTile = PathFinderTile.PathTiles.Find(x => x.Cords.Equals(Goal));
            Path.Add(PathTile.Cords);


            //PathFinderTile PathTile = PathFinderTile.PathTiles.Find(x => x.Cords.Equals(GoalTile.Carrier));
            //Path.Add(PathTile.Cords);

            //Console.WriteLine(PathFinderTile.PathTiles.Any(x => x.Cords.Equals(Tuple.Create(3, 4))));

            Itteration = 0;

            while (PathTile.Beginnin != true)
            {
                if (PathTile.Carrier == null)
                {
                    Console.WriteLine("CARRIER = 0");
                    return ImpossiblePath(StartPosition, Goal);

                }

                PathFinderTile PathTile1 = PathFinderTile.PathTiles.Find(x => x.Cords.Equals(PathTile.Carrier));
                PathTile = PathTile1;
                Path.Add(PathTile.Cords);

            }
            
            Path.Reverse();
            return Path;
        }

        public static bool IsBetween<T>(this T item, T start, T end)
        {
            return Comparer<T>.Default.Compare(item, start) >= 0
                && Comparer<T>.Default.Compare(item, end) <= 0;
        }

        private static List<Tuple<int, int>> ImpossiblePath(Tuple<int, int> StartPosition, Tuple<int, int> Goal)
        {
            Console.WriteLine("Paths impossible");

            int DifferenceX = StartPosition.Item1 - Goal.Item1;
            int DifferenceY = StartPosition.Item2 - Goal.Item2;
            List<Tuple<int, int>> Path = new List<Tuple<int, int>>();

            Tuple<int, int> CurrentCords = Tuple.Create(StartPosition.Item1, StartPosition.Item2);

            for (int x = 0; x < Math.Abs(DifferenceX); x++)
            {

                int xTemp;


                if (DifferenceX < 0)
                {
                    xTemp = StartPosition.Item1 + x;
                }
                else
                {
                    xTemp = StartPosition.Item1 - x;
                }

                Path.Add(Tuple.Create(xTemp, StartPosition.Item2));

            }
            for(int y = 0; y <= Math.Abs(DifferenceY); y++)
            {
                int yTemp;

                if (DifferenceY < 0)
                {
                    yTemp = StartPosition.Item2 + y;
                }
                else
                {
                    yTemp = StartPosition.Item2 - y;
                }

                Path.Add(Tuple.Create(Goal.Item1, yTemp));

            }

            foreach(var x in Path)
            {
                //Console.WriteLine("FAIL PATH: " + x);
            }
            return Path;

        }



    }

    class PathFinderTile
    {
        public Tuple<int,int> Cords { get; set; }
        public Tuple<int,int> North { get; set; }
        public Tuple<int, int> East { get; set; }
        public Tuple<int, int> South { get; set; }
        public Tuple<int, int> West { get; set; }

        public bool Beginnin { get; set; }
        public bool On { get; set; }
        public bool Filled { get; set; }
        public bool Tested { get; set; }
        public Tuple<int,int> Carrier { get; set; }

        public static List<PathFinderTile> PathTiles = new List<PathFinderTile>();

        public PathFinderTile(Tuple<int, int> Cords)
        {
            this.Cords = Cords;
            this.On = false;
            this.Beginnin = false;

            this.Carrier = null;
            this.Filled = false;
            this.Tested = false;

            this.North = Tuple.Create(Cords.Item1, Cords.Item2 - 1);
            this.East = Tuple.Create(Cords.Item1 + 1, Cords.Item2);
            this.South = Tuple.Create(Cords.Item1, Cords.Item2 + 1);
            this.West = Tuple.Create(Cords.Item1 - 1, Cords.Item2);
        }

        public static void CheckForPath()
        {
            foreach (PathFinderTile _Tile in PathTiles)
            {
                if (_Tile.On == true && _Tile.Tested == false)
                {
                    List<PathFinderTile> _TempList = new List<PathFinderTile>(PathTiles.FindAll(x => x.Cords.Equals(_Tile.South) || x.Cords.Equals(_Tile.North) || x.Cords.Equals(_Tile.East) || x.Cords.Equals(_Tile.West)));

                    foreach (PathFinderTile x in _TempList)
                    {
                        _Tile.Tested = true;

                        if (x.On != true)
                        {
                            x.On = true;
                            x.Carrier = _Tile.Cords;
                        }
                    }
                }
            }
        
        }


    }
}
