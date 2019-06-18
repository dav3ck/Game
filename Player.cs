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
    class Player
    {
        public int Speed { get; set; }
        public double Vertical_Horizontal_Speed { get; set; }
        public int Scale { get; set; }
        public bool Frozen { get; set; }
        public string Image { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public int XTile { get; set; }
        public int YTile { get; set; }

        public int chunck { get; set; }

        public List<Tiles> Tilelist { get; set; }

        public float HitboxWidthMargin { get; set; }
        public float HitboxHalf { get; set; }
        public float HitboxSize { get; set; }

        public int CenterCordsX { get; set; }
        public int CenterCordsY { get; set; }

        public int ImageCordsX { get; set; }
        public int ImageCordsY { get; set; }

        public Rectangle Hitbox;
        //public bool Rotation { get; set; } // false UP - DOWN $$ true LEFT - RIGHT

        public static float Horizontal_Momentum { get; set; }
        public static float Vertical_Momentum { get; set; }

        public Texture2D imgPlayer { get; set; }
        public SpriteBatch FrontSpriteBatch;

        public static Player Player1 {get;set;}

        private Autowalker Autowalk { get; set; }
        public Tuple<int, int> PrevTile { get; set; }

        public bool NoNPCIntersect { get; set; }

        private int AnimationDirection { get; set; }
        private int AnimationItteration { get; set; }
        private bool IsMoving { get; set; }
        private int AnimationCount { get; set; }
 


        public Player(int _Xtile, int _Ytile, SpriteBatch FrontSpriteBatch, PrologueContent prologueContent, List<Tiles> Tilelist)
        {

            LoadJson();

            this.XTile = _Xtile;
            this.YTile = _Ytile;

            imgPlayer = prologueContent.imgPlayer;
            this.Width = 16 * this.Scale;
            this.Height = 30 * this.Scale;

            this.CenterCordsX = (int)(this.XTile * Screen.GridSize + Screen.GridSize / 2);
            this.CenterCordsY = (int)(this.YTile * Screen.GridSize + Screen.GridSize / 2);

            this.chunck = 0;



            this.FrontSpriteBatch = FrontSpriteBatch;
            this.Tilelist = Map.Tilelist;

            this.HitboxSize = this.Width / 2;
            this.HitboxWidthMargin = (this.Width - this.HitboxSize) / 2;
            this.HitboxHalf = this.HitboxSize / 2;

            this.ImageCordsX = (int)(this.CenterCordsX - this.Width / 2);
            this.ImageCordsY = (int)(this.CenterCordsY + (this.HitboxHalf) - this.Height);

            Player1 = this;
            Chunck.Update();
            Update();
        }

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader("C:/Users/david/source/repos/Prologue/Prologue/Player.json"))
            {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);

                this.Speed = array.Speed;
                this.Vertical_Horizontal_Speed = array.Vertical_Horizontal_Speed;
                this.Scale = array.Scale;
                Frozen = array.Frozen;
            }
        }

        public void Draw()
        {
            Tuple<int, int> camera = Camera.GetCameraCords();
            //Vector2 ScreenCamera = new Vector2(Screen.CameraX, Screen.CameraY);

            SpriteSheet.screenDraw("Player_Walk_SpriteSheet", AnimationDirection, AnimationItteration, new Vector2((int)(this.ImageCordsX - camera.Item1), (int)(this.ImageCordsY - camera.Item2)), FrontSpriteBatch, (int)this.Width, (int)this.Height);

            //FrontSpriteBatch.Draw(imgPlayer, new Rectangle((int)(this.ImageCordsX - camera.Item1), (int)(this.ImageCordsY - camera.Item2), (int)this.Width, (int)this.Height), Color.White);
            FrontSpriteBatch.Draw(Game1.prologueContent.Testbox_Sprite, new Rectangle((int)(Hitbox.X - camera.Item1), (int)(Hitbox.Y - camera.Item2), (int)this.HitboxSize, (int)this.HitboxSize), Color.Red);
            
        }


        public void HorizontalMov(string Direction)
        {
            //If player Input is given it wil test for the direct pressed if the Player is actually allowed to mvoe that way. 
            // Horizontal Movement & vertical Movement are split so when you can no longer walk up, but u stil have the Left key pressed it wil continue wall hugging to the left

            if (Frozen == true) { return; }

            if (Direction == "Left")
            {
                Horizontal_Momentum = -1 * Speed;
            }
            else if (Direction == "Right")
            {
                Horizontal_Momentum = Speed; 
            }

            //PlayerMoves();
        }

        public void VerticalMov(string Direction)
        {
            if (Frozen == true) { return; }

            if (Direction == "Up")
            {
                Vertical_Momentum = -1 * (float)Speed * (float)Vertical_Horizontal_Speed;
            }
            else if (Direction == "Down")
            {
                Vertical_Momentum = 1 * (float)Speed * (float)Vertical_Horizontal_Speed;
            }

                //PlayerMoves();
        }

        public void PlayerMoves()
        {
            IsMoving = true;

            int _Xtemp = this.CenterCordsX + (int)Horizontal_Momentum;
            int _Ytemp = this.CenterCordsY + (int)Vertical_Momentum;

            Animation((int)Vertical_Momentum, (int)Horizontal_Momentum);

            //THIS ALL WAS USED FOR TURNING THE HITBOXES, DECIDED TO SQUAR HITBOX TO MAKE THINGS EASIER WITH ROTATING

            //bool _TempRotation = Autowalker.CalculateFaceDirection(Tuple.Create(Horizontal_Momentum, Vertical_Momentum));

            /* bool Canturn = TestHitBoxCollision(GetRectangle(this.CenterCordsX, this.CenterCordsY, _TempRotation)); // TEST whether or not player can turn,

             if (Canturn == true)
             {
                 if(this.Rotation == false && Vertical_Momentum != 0)
                 {
                     this.Rotation = false;
                 }
                 else
                 {
                     this.Rotation = _TempRotation;
                 }
             }*/




            bool CanHorwalk = TestHitBoxCollision(GetRectangle(_Xtemp, this.CenterCordsY,(int)this.HitboxSize)); // TEST whether or not Runs into solid object
            bool CanVerwalk = TestHitBoxCollision(GetRectangle(this.CenterCordsX, _Ytemp,(int)this.HitboxSize));


            if (CanHorwalk != true) { Horizontal_Momentum = 0; }
            if (CanVerwalk != true) { Vertical_Momentum = 0; }

            Update();

            
        }

        public bool TestHitBoxCollision(Rectangle TempHitbox)
        {
           /* Tuple<int, int> North = Tuple.Create(TempHitbox.Top, TempHitbox.Center.X);
            Tuple<int, int> East = Tuple.Create(TempHitbox.Right, TempHitbox.Center.Y);
            Tuple<int, int> South = Tuple.Create(TempHitbox.Bottom, TempHitbox.Center.X);
            Tuple<int, int> West = Tuple.Create(TempHitbox.Left, TempHitbox.Center.Y); */

            Tuple<int, int> NorthWest = Tuple.Create(TempHitbox.Left, TempHitbox.Top);
            Tuple<int, int> NorthEast = Tuple.Create(TempHitbox.Right, TempHitbox.Top);
            Tuple<int, int> SouthEast = Tuple.Create(TempHitbox.Right, TempHitbox.Bottom);
            Tuple<int, int> SouthWest = Tuple.Create(TempHitbox.Left, TempHitbox.Bottom);

            Tuple<int, int> NorthWestGrid = Screen.GridCords(NorthWest.Item1, NorthWest.Item2);
            Tuple<int, int> NorthEastGrid = Screen.GridCords(NorthEast.Item1, NorthEast.Item2);
            Tuple<int, int> SouthEastGrid = Screen.GridCords(SouthEast.Item1, SouthEast.Item2);
            Tuple<int, int> SouthWestGrid = Screen.GridCords(SouthWest.Item1, SouthWest.Item2);

            if (SolidTile.LoadedSolidTiles.Any(x => x.Cords.Equals(NorthWestGrid) || x.Cords.Equals(SouthWestGrid) || x.Cords.Equals(NorthEastGrid) || x.Cords.Equals(SouthEastGrid)))
            {
                return false;
            }

            foreach(NPC x in NPC.NPClist)
            {
                //Console.WriteLine("NPC hitbox " + x.Hitbox.Location + " - Size " + x.Hitbox.Size);
                if (NoNPCIntersect)
                {
                    break;
                }
                //Console.WriteLine(NorthWest + " " + NorthEast + " " + SouthEast + " " + SouthWest);

                if (x.Hitbox.Contains(NorthWest.Item1,NorthWest.Item2) || x.Hitbox.Contains(NorthEast.Item1, NorthEast.Item2) || x.Hitbox.Contains(SouthEast.Item1, SouthEast.Item2) || x.Hitbox.Contains(SouthWest.Item1, SouthWest.Item2))
                {
                    return false;
                }
            }

            return true;
        }

        public void Update()
        {

            this.CenterCordsX += (int)Horizontal_Momentum;
            this.CenterCordsY += (int)Vertical_Momentum;

            this.Hitbox = GetRectangle(this.CenterCordsX, this.CenterCordsY, (int)this.HitboxSize);

            Tuple<int, int> GridCords = Screen.GridCords(this.CenterCordsX, this.CenterCordsY);

            if(XTile != GridCords.Item1 || YTile != GridCords.Item2)
            {
                Console.WriteLine(GridCords);
                PrevTile = Tuple.Create(XTile, YTile);

                this.XTile = GridCords.Item1;
                this.YTile = GridCords.Item2;

                foreach (NPC x in NPC.NPClist)
                {
                    if (x.Follow)
                    {
                        x.Follower();
                    }
                }

            }

            int Cur_Chunck = Chunck.CurrentChunck(this.YTile);

            if (this.chunck != Cur_Chunck)
            {
                this.chunck = Cur_Chunck;
                Chunck.Update();
            }


            EventZone.PlayerIntersection(this.YTile);

            //Screen.CameraMovement(this.CenterCordsX, this.CenterCordsY);

            Horizontal_Momentum = 0;
            Vertical_Momentum = 0;

            this.ImageCordsX = (int)(this.CenterCordsX - this.Width / 2);
            this.ImageCordsY = (int)(this.CenterCordsY + (this.HitboxHalf) - this.Height);



            foreach (Objects x in SolidTile.LoadedObjects)
            {
                x.Update(this.Hitbox);
            }

            Player1 = this;
        }

        public void TickUpdate()
        {
            if (Vertical_Momentum != 0 || Horizontal_Momentum != 0)
            {
                PlayerMoves();
            }
            
            if (!IsMoving)
            {
                AnimationItteration = 0;
            }

            IsMoving = false;
        }

        public static Rectangle GetRectangle(int _CenterCordX, int _CenterCordY, int _HitboxSize)
        {
            int _HitBoxtopCordsX = (int)(_CenterCordX - _HitboxSize / 2);
            int _HitBoxTopCordsY = (int)(_CenterCordY - _HitboxSize / 2);

            return (new Rectangle(_HitBoxtopCordsX, _HitBoxTopCordsY, _HitboxSize, _HitboxSize));

            /*else
            {
                return (new Rectangle((int)(_CenterCordX - this.HitboxHalf), (int)(_CenterCordY - this.HitboxHalf), (int)this.HitboxSize, (int)this.HitboxSize));
            }*/
        }


        public static void MovePlayer(Tuple<int,int> Momentum)
        {
            Player1.IsMoving = true;
            Player1.Animation(Momentum.Item2, Momentum.Item1);

            Player1.CenterCordsX += Momentum.Item1;
            Player1.CenterCordsY += Momentum.Item2;

            Player1.Update();
        }

        /*public bool AllowMovement(Tuple<float,float> cord1, Tuple<float, float> cord2)
        {
            Tuple<int, int> GridCord1 = Screen.GridCords(cord1.Item1, cord1.Item2);
            Tuple<int, int> GridCord2 = Screen.GridCords(cord2.Item1, cord2.Item2);

            Rectangle RecCord1 = new Rectangle((int)cord1.Item1, (int)cord1.Item2,1,1);
            Rectangle RecCord2 = new Rectangle((int)cord2.Item1, (int)cord2.Item2,1,1);

            foreach (var tile in Tilelist)
            {
                if (tile.X == GridCord1.Item1 && tile.Y == GridCord1.Item2 && tile.Solid == true)
                {
                    return false;
                    
                }
                else if (tile.X == GridCord2.Item1 && tile.Y == GridCord2.Item2 && tile.Solid == true)
                {
                    return false;
                }
            }

            foreach (var _Object in Objects.ObjectList)
            {
                if (_Object.Xtile == GridCord1.Item1 && _Object.Ytile == GridCord1.Item2 && _Object.Solid == true)
                {
                    return false;
                }
                else if (_Object.Xtile == GridCord2.Item1 && _Object.Ytile == GridCord2.Item2 && _Object.Solid == true)
                {
                    return false;
                }
            }

            foreach (var NPC in NPC.NPClist)
            {
                if (RecCord1.Intersects(NPC.Hitbox))
                {
                    return false;
                }
                if (RecCord2.Intersects(NPC.Hitbox))
                {
                    return false;
                }
            }  
            return true; 
        }

        public bool AllowMovementNPC(Tuple<float,float> cord1, Tuple<float,float> cord2)
        {
            return true;
        } */

        private void Animation(int Vertical_Momentum, int Horizontal_Momentum)
        {
            if (Vertical_Momentum > 0)
            {
                AnimationDirection = 3;
            }
            else if (Vertical_Momentum < 0)
            {
                AnimationDirection = 0;
            }
            else if (Horizontal_Momentum > 0)
            {
                AnimationDirection = 2;
            }
            else if (Horizontal_Momentum < 0)
            {
                AnimationDirection = 1;
            }

            AnimationCount++;

            if (AnimationCount % 5 == 0)
            {
                AnimationItteration++;
            }

            if (AnimationItteration > 3)
            {
                AnimationItteration = 0;
            } 
        }

        public static Tuple<int, int> GetPlayerCords(Player x) // HIERNAAR KIJKEN
        {
            return Tuple.Create(x.CenterCordsX, x.CenterCordsY);
        }

        public static Tuple<int,int> GetPlayerTiles(Player x)
        {
            return Tuple.Create(x.XTile, x.YTile);
        }

        public Tuple<int,int> GetPrevTile()
        {
            return this.PrevTile;
        }

        public static void FreezePlayer(bool x) // FROZEN IS STATIC VARIABLE NIET NETJES!!!!!
        {
            Player1.Frozen = x;
        }

        public static void PlayerInitializeAutoWalk(Tuple<int,int> _goal, bool _wait)
        {
            Tuple<int, int> CurrentPosition = Screen.GridCords(Player1.CenterCordsX, Player1.CenterCordsY);
            Console.WriteLine("TEJFKJFLKJ  " + CurrentPosition);
            List<Tuple<int,int>> _path = Utility.GeneratePath(CurrentPosition, _goal);
            Player1.Autowalk = new Autowalker("Player", _path, Tuple.Create((float)Player1.CenterCordsX, (float)Player1.CenterCordsY), Tuple.Create(Player1.Speed, (int)(Player1.Speed * Player1.Vertical_Horizontal_Speed)), _wait);
        }

    }
}
