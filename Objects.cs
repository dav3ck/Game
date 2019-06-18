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
    class Objects
    {
        public int Xtile { get; set; }
        public int Ytile { get; set; }
        public int chunck { get; set; }

        public float Xcord { get; set; }
        public float Ycord { get; set; }

        public bool Solid { get; set; }
        public int ID { get; set; }

        public string Information { get; set; }
        public string Name { get; set; }
        public int EventID { get; set; }

        public SpriteBatch FrontSpriteBatch { get; set; }
        public PrologueContent prologueContent { get; set; }
        public Texture2D Objectimg { get; set; }

        public bool CanInteract { get; set; }

        public static List<Objects> ObjectList = new List<Objects>();

        private AnimationTick Animation { get; set; }
        private bool IsAnimated { get; set; }
        private string SpriteSheetName { get; set; }
        private int AnimationIndex { get; set; }

        //------------------------------------------------------------

        public Objects(int _Xtile, int _Ytile, int ID)
        {
            SpriteSheetName = "Test_Animation_SpriteSheet";
            this.Xtile = _Xtile;
            this.Ytile = _Ytile;

            this.prologueContent = Game1.prologueContent;

            LoadObjectData(ID);

            this.chunck = Chunck.CurrentChunck(this.Ytile);

            this.FrontSpriteBatch = Game1.FrontSpriteBatch;
            Tuple<int, int> _Screencords = Screen.ScreenCords(this.Xtile, this.Ytile);
            this.Xcord = _Screencords.Item1;
            this.Ycord = _Screencords.Item2;

            if (this.Solid == true)
            {
                SolidTile.AllSolidTiles.Add(new SolidTile(Tuple.Create(Xtile, Ytile), chunck));
            }

            IsAnimated = SpriteSheet.IsAnimated(SpriteSheet.GetSpriteSheet(SpriteSheetName));
            if (IsAnimated == true)
            {
                Animation = AnimationTick.GetAnimationTick(SpriteSheetName, 0);
                AnimationIndex = Animation.AssignAnimationIndex();
            }
            
            //ObjectList.Add(this);
        }

        private void LoadObjectData(int ID)
        {
            var json = File.ReadAllText("C:/Users/david/source/repos/Prologue/Prologue/Objects.Json");
            try
            {
                var jObject = JObject.Parse(json);
                if (jObject == null) {  return; }
                JArray ObjectArray = (JArray)jObject["Objects"];
                if (ObjectArray == null) { return; }

                foreach (var item in ObjectArray)
                {
                    if (ID == (int)item["ID"])
                    {
                        this.ID = (int)item["ID"];
                        this.Name = item["Name"].ToString();
                        this.Solid = (bool)item["Solid"];
                        this.CanInteract = (bool)item["CanInteract"];
                        this.Information = item["Information"].ToString();
                        this.Objectimg = (Texture2D)prologueContent.GetType().GetProperty(item["Texture"].ToString()).GetValue(prologueContent);
                        this.EventID = (int)item["EventID"];
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void Update(Rectangle PlayerHitbox)
        {

            //Collision with player is tested here (whether or not the object is interactable)
            //It Makes a new Rectangle slightly out of the middle of the Hitbox of the Object
            //Then checks whether or not this hitbox collides with the Player Hitbox

            CanInteract = false;

            Rectangle _HitboxObject = new Rectangle((int)this.Xcord, (int)this.Ycord, (int)Screen.GridSize, (int)Screen.GridSize);

            Rectangle _Side1 = new Rectangle((int)this.Xcord - 20, (int)this.Ycord + _HitboxObject.Height / 2, 3, 3);
            Rectangle _Side2 = new Rectangle((int)this.Xcord + _HitboxObject.Width / 2, (int)this.Ycord - 20, 3, 3);
            Rectangle _Side3 = new Rectangle((int)this.Xcord + 20 + _HitboxObject.Width, (int)this.Ycord + _HitboxObject.Height / 2, 3, 3);
            Rectangle _Side4 = new Rectangle((int)this.Xcord + _HitboxObject.Width / 2, (int)this.Ycord + 10 + _HitboxObject.Height, 3, 3);

            if(_Side1.Intersects(PlayerHitbox) || _Side2.Intersects(PlayerHitbox) || _Side3.Intersects(PlayerHitbox) || _Side4.Intersects(PlayerHitbox))
            {
                CanInteract = true;
            }

            //Temperary Visualisation of this

            if (CanInteract == true)
            {
                this.Objectimg = prologueContent.Tile2;
            }
            else
            {
                this.Objectimg = prologueContent.Tile1;
            }
        } 

        public void Draw()
        {
            int x = 1;
            if (IsAnimated)
            {
                x = Animation.GetIterationIndexValue(AnimationIndex);
            }

            SpriteSheet.screenDraw("Test_Animation_SpriteSheet", 0, x, new Vector2((int)(this.Xcord - Screen.CameraX), (int)(this.Ycord - Screen.CameraY)), FrontSpriteBatch, (int)Screen.GridSize, (int)Screen.GridSize);
        
        }

        public void Interact()
        {
            if (EventID != 0)
            {
                EventHandler.EventList.Add(new EventHandler(EventID));
            }
            else
            {
                Textbox.TextBoxes.Add(new InformationTextBox(this.Information, "Object"));
            }
        }

        public static void RemoveObj(List<Tuple<int,int>> _ObjectTiles)
        {
            /*List<Objects> _RemoveObjects = new List<Objects>();

            foreach (Tuple<int,int> y in _ObjectTiles)
            {
                Objects _object = Objects.ObjectList.Find(x => x.Xtile == y.Item1 && x.Ytile == y.Item2);
                if (_object != null)
                {
                    _RemoveObjects.Add(_object);
                }
            } */

            /*foreach (var z in _RemoveObjects)
            {
                Objects.ObjectList.Remove(z);
            }*/

            /*List<SolidTile> _temp = new List<SolidTile>();

            foreach(var z in _RemoveObjects)
            {
                _temp = SolidTile.AllSolidTiles.FindAll(x => x.Cords.Equals(Tuple.Create(z.Xtile, z.Ytile)));
                _temp.ForEach(x => SolidTile.AllSolidTiles.Remove(x));
            } */

            _ObjectTiles.ForEach(y => Objects.ObjectList.Remove(Objects.ObjectList.Find(x => x.Xtile == y.Item1 && x.Ytile == y.Item2)));
            _ObjectTiles.ForEach(y => SolidTile.AllSolidTiles.Remove(SolidTile.AllSolidTiles.Find(x => x.Cords.Equals(y))));

            Chunck.Update();
        }

        public static void CreateObj(List<int[]> _objects)
        {
             _objects.ForEach(x => Objects.ObjectList.Add(new Objects(x[0], x[1], x[2])));
            Chunck.Update();
        }
    }


    //Eventually certain Objects can do Certain things, So I made the main object class with properties all objects share, and then wil have inheritand classes for the special features, Like a computer that can load


}
