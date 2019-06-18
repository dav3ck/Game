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
    static class Camera
    {
        public static int Xcord { get; private set; }
        public static int Ycord { get; private set; }

        public static Vector2 Location { get; private set; }

        public static Tuple<int,int> Tilecord { get; private set; }

        public static bool LockOnPlayer { get; set; }
        public static bool InCinematic { get; private set; }
        public static bool ReturnToOriginal { get; private set; }

        public static Vector2 Finish { get; private set; }
        public static Vector2 Speed { get; private set; }
        public static Vector2 Direction { get; private set; }
        public static Vector2 ToTravel { get; private set; }
        private static string Caller { get; set; }

        public static void Update()
        {
            if(Camera.LockOnPlayer == true)
            {
                PlayerCenter();
            }
            else if(Camera.InCinematic == true)
            {
                Cinematic();
            }

            Screen.CameraX = Camera.Location.X;
            Screen.CameraY = Camera.Location.Y;
        }

        private static void PlayerCenter()
        {
            Tuple<int, int> PlayerCords = Player.GetPlayerCords(Player.Player1);
            Camera.Location = new Vector2(PlayerCords.Item1 - (int)Screen.ScreenWidth / 2, PlayerCords.Item2 - (int)Screen.ScreenHeight / 2);
            /*Camera.Xcord = PlayerCords.Item1 - (int)Screen.ScreenWidth / 2;
            Camera.Ycord = PlayerCords.Item2 - (int)Screen.ScreenHeight / 2;*/
        }

        public static void InitializeCinematic(Tuple<int,int> _Goal, int _speed, bool _return, string _caller)
        {
            Camera.ReturnToOriginal = _return;
            Camera.Caller = _caller;

            Player.FreezePlayer(true);

            Tuple<int, int> LocationTile = Screen.GridCords(Location.X, Location.Y);
            Tuple<int, int> _Goalcords = Screen.ScreenCords(_Goal.Item1 + LocationTile.Item1, _Goal.Item2 + LocationTile.Item2);

            if (ReturnToOriginal == false)
            {
                Camera.Finish = new Vector2(_Goalcords.Item1, _Goalcords.Item2);
            }
            else { Camera.Finish = Camera.Location; }

            Tuple<int, int> InformationX = CalculateDirection(Camera.Location.X, _Goalcords.Item1);
            Tuple<int, int> InformationY = CalculateDirection(Camera.Location.Y, _Goalcords.Item2);

            Camera.ToTravel = new Vector2(InformationX.Item1, InformationY.Item1);

            float speedDifference;

            try
            {
                speedDifference = (ToTravel.X / ToTravel.Y);
            }
            catch (Exception)
            {
                speedDifference = 0;
            }

            Camera.Speed = new Vector2(_speed * speedDifference, _speed);
            Camera.Direction = new Vector2(InformationX.Item2, InformationY.Item2);

            Camera.LockOnPlayer = false;
            Camera.InCinematic = true;
        }

        private static void Cinematic()
        {
            //Console.WriteLine("Beforespeed " + ToTravel);
            Camera.ToTravel -= Camera.Speed;
           // Console.WriteLine("Speed " + Camera.Speed);

            if (ToTravel.X < 0 && ToTravel.Y < 0)
            {
                Camera.InCinematic = false;

                Camera.Location = Camera.Finish;
                if(Camera.ReturnToOriginal == true)
                {
                    Camera.LockOnPlayer = true;
                }

                if (Camera.Caller == "Event")
                {
                    EventHandler.Continue = true;
                }
            }
            else
            {
                Console.WriteLine(Camera.Direction);
                Camera.Location += (Camera.Speed * Camera.Direction);
                //Console.WriteLine(" Camera Location " + Camera.Location);
            }
            
        }

        public static Tuple<int,int> CalculateDirection(float _CurrentCord, float _GoalCord)
        {
            float Direction;
            float Distance;

            Distance = Math.Abs(_GoalCord - _CurrentCord);

            if (Distance != 0)
            {
                Direction = (_GoalCord - _CurrentCord) / Distance;
            }
            else { Direction = 0; }

            return (Tuple.Create((int)Distance, (int)Direction));
        }

        /*public static void SetCameraSpeed(int _speed)
        {
            Camera.Speed = _speed;
        } */

        public static void SetInCinematic(bool _bool)
        {
            Camera.InCinematic = _bool;
        }

        public static Tuple<int,int> GetCameraCords()
        {
            return (Tuple.Create((int)Camera.Location.X, (int)Camera.Location.Y));
        }

        public static void InitializeCamera(Vector2 _Location, bool _onPlayer)
        {
            Camera.Location = _Location;
            Camera.LockOnPlayer = _onPlayer;

            //Camera.Update();
        }
    }
}
