using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Prologue
{
    class NPC
    {
        public float ImageCordX { get; set; }
        public float ImageCordY { get; set; }

        public int CenterCordsX { get; set; }
        public int CenterCordsY { get; set; }

        public int Xtile { get; set; }
        public int Ytile { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public float HitboxHalf { get; set; }
        public float HitboxSize { get; set; }
        public float HitboxWidthMargin { get; set; }

        public List<Tiles> Tilelist { get; set; }

        public int scale { get; set; }
        public int speed { get; set; }
        public float Vertical_Horizontal_Speed { get; set; }
        public Rectangle Hitbox;

        public Texture2D NPCimg { get; set; }
        private SpriteBatch FrontSpriteBatch;
        PrologueContent prologueContent { get; set; }
        public string Name { get; set; }
        public bool Follow { get; set; }

        private Autowalker Autowalk { get; set; }

        static public List<NPC> NPClist = new List<NPC>();

        public NPC(int Xtile, int Ytile, string Name, SpriteBatch FrontSpriteBatch, PrologueContent prologueContent)
        {
            this.Xtile = Xtile;
            this.Ytile = Ytile;

            this.prologueContent = prologueContent;
            this.Name = Name;
            GetNPCData(this.Name);

            this.FrontSpriteBatch = FrontSpriteBatch;

            this.Tilelist = Map.Tilelist; // HAAL DIT NOG WEG?

            this.Width = NPCimg.Width * this.scale;
            this.Height = NPCimg.Height * this.scale;

            this.CenterCordsX = (int)(this.Xtile * Screen.GridSize + Screen.GridSize / 2);
            this.CenterCordsY = (int)(this.Ytile * Screen.GridSize + Screen.GridSize / 2);

            this.HitboxSize = this.Width / 2;
            this.HitboxWidthMargin = (this.Width - this.HitboxSize) / 2;
            this.HitboxHalf = this.HitboxSize / 2;

            NPClist.Add(this);

            Update();

        }

        private void GetNPCData(string Name)
        {
            var Json = File.ReadAllText("C:/Users/david/source/repos/Prologue/Prologue/NPC_" + Name + ".json");

            try
            {
                var jObject = JObject.Parse(Json);
                if (jObject != null)
                {
                    this.speed = (int)jObject["Speed"];
                    this.scale = (int)jObject["Scale"];
                    this.Vertical_Horizontal_Speed = (float)jObject["Vertical_Horizontal_Speed"];
                    this.NPCimg = (Texture2D)prologueContent.GetType().GetProperty(jObject["Texture"].ToString()).GetValue(prologueContent);
                    Console.WriteLine(this.NPCimg);
                }
            }
            catch (Exception)
            {
            }
        }

        public void Draw()
        {
            FrontSpriteBatch.Draw(NPCimg, new Rectangle((int)(this.ImageCordX - Screen.CameraX), (int)(this.ImageCordY - Screen.CameraY), (int)this.Width, (int)this.Height), Color.White);
            FrontSpriteBatch.Draw(Game1.prologueContent.Testbox_Sprite, new Rectangle((int)(Hitbox.X - Screen.CameraX), (int)(Hitbox.Y - Screen.CameraY), (int)this.HitboxSize, (int)this.HitboxSize), Color.Red);
        }

        private void Update()
        {

            this.ImageCordX = (int)(this.CenterCordsX - this.Width / 2);
            this.ImageCordY = (int)(this.CenterCordsY + (this.HitboxHalf) - this.Height);

            Tuple<int, int> Tilecords = Screen.GridCords(this.CenterCordsX, this.CenterCordsY);
            this.Xtile = Tilecords.Item1;
            this.Ytile = Tilecords.Item2;

            this.Hitbox = Player.GetRectangle(this.CenterCordsX, this.CenterCordsY, (int)this.HitboxSize);

            /*
            if (this.Follow)
            {

                List<Tuple<int, int>> _path = Utility.GeneratePath(Tilecords, Player.Player1.GetPrevTile());
                Autowalk = new Autowalker(this.Name, _path, Tuple.Create(CenterCordsX + (this.HitboxHalf), CenterCordsY - (this.HitboxHalf)), Tuple.Create(this.speed, (int)(this.speed * this.Vertical_Horizontal_Speed)), false);
            } */
        }

        public static void WalkNPC(string _name, Tuple<int,int> Momentum)
        {
            NPC npc = NPC.NPClist.Find(x => x.Name == _name);

            npc.CenterCordsX += Momentum.Item1;
            npc.CenterCordsY += Momentum.Item2;

            npc.Update();
        }

        public void InitializeWalk(Tuple<int,int> Goal, bool _wait)
        {
            Tuple<int, int> CurrentPosition = Screen.GridCords(CenterCordsX,CenterCordsY);
            List<Tuple<int, int>> _path = Utility.GeneratePath(CurrentPosition, Goal);

            Autowalk = new Autowalker(this.Name, _path, Tuple.Create(CenterCordsX + (this.HitboxHalf),CenterCordsY - (this.HitboxHalf)), Tuple.Create(this.speed,(int)( this.speed * this.Vertical_Horizontal_Speed)), _wait);
        }

        public void Follower()
        {
            Autowalker.ManualDeleteAutoWalker(Autowalk);

            List<Tuple<int, int>> _path = Utility.GeneratePath(Tuple.Create(this.Xtile,this.Ytile), Player.Player1.GetPrevTile());
            Autowalk = new Autowalker(this.Name, _path, Tuple.Create(CenterCordsX + (this.HitboxHalf), CenterCordsY - (this.HitboxHalf)), Tuple.Create(this.speed, (int)(this.speed * this.Vertical_Horizontal_Speed)), false);
        }

        public static void RemoveNPC(string _Name)
        {
            NPClist.RemoveAll(x => x.Name == _Name);
        }

        /*public static Tuple<int,int> GetNPCCords(string _name)
        {
            try
            {
                NPC _npc = NPC.NPClist.Find(x => x.Name == _name);
                return (Tuple.Create(_npc.CenterCordsX, _npc.CenterCordsY));
            }
            catch (Exception)
            {
                Console.WriteLine("Name not recognized, Returning a value of 0,0");
                return (Tuple.Create(0, 0));
            }
        }*/

        public Tuple<int,int> GetNPCCords()
        {
            return (Tuple.Create(this.CenterCordsX, this.CenterCordsY));
        }

        public static void ToggleFollow(string _name)
        {
            NPC npc = NPC.NPClist.Find(x => x.Name == _name);
            if (npc == null)
            {
                Console.WriteLine("Invalid Name!");
                return;
            }

            Player.Player1.NoNPCIntersect = !Player.Player1.NoNPCIntersect;
            npc.Follow = !npc.Follow;
        }
    }
}

        /*public void CalculatePath(int XGoal, int YGoal)
        {
            //THIS DOESNT WORK AND WIL NOT BE USED

            this.TilesWalked = 0;
            this.Moving = true;

            List<PathData> WrongTiles = new List<PathData>();

            int DirectionX = 0;
            String S_DirectionX = "";
            int DirectionY = 0;
            String S_DirectionY = "";
            string Direction = "";

            int CurrentX = this.Xtile;
            int CurrentY = this.Ytile;

            int NextX = CurrentX;
            int NextY = CurrentY;

            Path.Add(new PathData(CurrentX, CurrentY));
            

            int AmountX = this.Xtile - XGoal;
            int AmountY = this.Ytile - YGoal;

            if (AmountX >= 0) { DirectionX = -1; S_DirectionX = "Left"; }
            else if (AmountX < 0) { DirectionX = 1; S_DirectionX = "Right"; }
            //else { DirectionX = 0; }
            if (AmountY >= 0) { DirectionY = -1; S_DirectionY = "Up"; }
            else if (AmountY < 0) { DirectionY = 1; S_DirectionY = "Down"; }
            //else { DirectionY = 0; }

            AmountX = Math.Abs(AmountX);
            AmountY = Math.Abs(AmountY);

            

            while (AmountX != 0 || AmountY != 0)
            {
                bool TileLeft = (Tilelist.Any(x => x.Y == CurrentY && x.X == (CurrentX + -1) && x.Solid == false) && !WrongTiles.Any(x => x.Y == CurrentY && x.X == (CurrentX + -1)) && !Path.Any(x => x.Y == CurrentY && x.X == (CurrentX + -1)));
                bool TileRight = (Tilelist.Any(x => x.Y == CurrentY && x.X == (CurrentX + 1) && x.Solid == false) && !WrongTiles.Any(x => x.Y == CurrentY && x.X == (CurrentX + 1)) && !Path.Any(x => x.Y == CurrentY && x.X == (CurrentX + 1)));
                bool TileUp = (Tilelist.Any(x => x.Y == (CurrentY + -1) && x.X == CurrentX && x.Solid == false) && !WrongTiles.Any(x => x.Y == (CurrentY + -1) && x.X == CurrentX) && !Path.Any(x => x.Y == (CurrentY + -1) && x.X == CurrentX)) ;
                bool TileDown = (Tilelist.Any(x => x.Y == (CurrentY + 1) && x.X == CurrentX && x.Solid == false) && !(WrongTiles.Any(x => x.Y == (CurrentY + 1) && x.X == CurrentX)) && !Path.Any(x => x.Y == (CurrentY + 1) && x.X == CurrentX));

                Console.WriteLine(TileLeft + " " + TileRight + " " + TileUp + " " + TileDown);

                if (TileLeft == false && TileRight == false && TileUp == false && TileDown == false)
                {
                    TileLeft = (Tilelist.Any(x => x.Y == CurrentY && x.X == (CurrentX + -1) && x.Solid == false) && !WrongTiles.Any(x => x.Y == CurrentY && x.X == (CurrentX + -1)));
                    TileRight = (Tilelist.Any(x => x.Y == CurrentY && x.X == (CurrentX + 1) && x.Solid == false) && !WrongTiles.Any(x => x.Y == CurrentY && x.X == (CurrentX + 1)));
                    TileUp = (Tilelist.Any(x => x.Y == (CurrentY + -1) && x.X == CurrentX && x.Solid == false) && !WrongTiles.Any(x => x.Y == (CurrentY + -1) && x.X == CurrentX));
                    TileDown = (Tilelist.Any(x => x.Y == (CurrentY + 1) && x.X == CurrentX && x.Solid == false) && !(WrongTiles.Any(x => x.Y == (CurrentY + 1) && x.X == CurrentX)));

                }

                // Console.WriteLine(AmountX);


                if (AmountX >= AmountY)
                {
                    if ((S_DirectionX == "Left" && TileLeft == true) || (S_DirectionX == "Right" && TileRight == true))
                    {
                        Direction = S_DirectionX;
                        AmountX -= 1;
                    }
                    else if ((S_DirectionY == "Up" && TileUp == true) || (S_DirectionY == "Down" && TileDown == true))
                    {
                        Direction = S_DirectionY;
                        AmountY -= 1;
                    }
                    else if ((S_DirectionY == "Up" && TileDown == true) || (S_DirectionY == "Down" && TileUp == true))
                    {
                        if (S_DirectionY == "Up" && TileDown == true) { Direction = "Down"; }
                        else if (S_DirectionY == "Down" && TileUp == true) { Direction = "Up"; }
                        AmountY += 1;
                    }
                    else if ((S_DirectionX == "Left" && TileRight == true) || (S_DirectionX == "Right" && TileLeft == true))
                    {
                        if (S_DirectionX == "Left" && TileRight == true) { Direction = "Right"; }
                        else if (S_DirectionX == "Right" && TileLeft == true) { Direction = "Left"; }
                        AmountX += 1;
                    }

                }
                else if (AmountY > AmountX)
                {
                    if ((S_DirectionY == "Up" && TileUp == true) || (S_DirectionY == "Down" && TileDown == true))
                    {
                        Direction = S_DirectionY;
                        AmountY -= 1;
                    }
                    else if ((S_DirectionX == "Left" && TileLeft == true) || (S_DirectionX == "Right" && TileRight == true))
                    {
                        Direction = S_DirectionX;
                        AmountX -= 1;
                    }
                    else if ((S_DirectionX == "Left" && TileRight == true) || (S_DirectionX == "Right" && TileLeft == true))
                    {
                        if (S_DirectionX == "Left" && TileRight == true) { Direction = "Right"; }
                        else if (S_DirectionX == "Right" && TileLeft == true) { Direction = "Left"; }
                        AmountX += 1;
                    }
                    else if ((S_DirectionY == "Up" && TileDown == true) || (S_DirectionY == "Down" && TileUp == true))
                    {
                        if (S_DirectionY == "Up" && TileDown == true) { Direction = "Down"; }
                        else if (S_DirectionY == "Down" && TileUp == true) { Direction = "Up"; }
                        AmountY += 1;
                    }
                }

                if (Direction == "Left" || Direction == "Right")
                {
                    NextX = CurrentX + DirectionX;
                }
                else if(Direction == "Up" || Direction == "Down")
                {
                    NextY = CurrentY + DirectionY;
                }

                if (Path.Any(x => x.X == NextX && x.Y == NextY))
                {
                    WrongTiles.Add(new PathData(CurrentX, CurrentY));
                    int Index = Path.FindIndex(x => x.X == NextX && x.Y == NextY);
                    Path = new List<PathData>(Path.Take(Index));
                }
                else
                {
                    Path.Add(new PathData(NextX, NextY));
                }


                if (AmountX < 0)
                {
                    AmountX = Math.Abs(AmountX);
                    if (S_DirectionX == "Left")
                    {
                        S_DirectionX = "Right";
                        DirectionX = 1;
                    }
                    else
                    {
                        S_DirectionX = "Left";
                        DirectionX = -1;
                    }
                }
                if (AmountY < 0)
                {
                    AmountY = Math.Abs(AmountY);
                    if (S_DirectionY == "Down")
                    {
                        S_DirectionY = "Up";
                        DirectionY = -1;
                    }
                    else
                    {
                        S_DirectionY = "Down";
                        DirectionY = 1;
                    }
                }
   
                CurrentX = NextX;
                CurrentY = NextY;

                Console.WriteLine(CurrentX + "  --  " + CurrentY);



            }

            foreach( var Tile in Path)
            {
                //Console.WriteLine(Tile.X + " -- " + Tile.Y);
            }

        }



    }


}
        


        public struct PathData
        {
            public PathData(int X, int Y)
            {
                this.X = X;
                this.Y = Y;
            }

            public int X { get; private set; }
            public int Y { get; private set; }
        }
        */
