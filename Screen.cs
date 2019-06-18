using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prologue
{
    public static class Screen
    {
        static public float ScreenWidth { get; set; }
        static public float ScreenHeight { get; set; }

        static public int MinGridX = 16;
        static public int MinGridY = 9;

        static public float GridSize { get; set; }

        static public float CameraX { get; set; }
        static public float CameraY { get; set; }

        static public Tuple<int, int> ScreenCords(int x, int y)
        {
            //Console.WriteLine(GridSize);
            //Console.WriteLine(x * GridSize + " " + y * GridSize);
            return Tuple.Create((int)(x * GridSize), (int)(y * GridSize));
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
}
