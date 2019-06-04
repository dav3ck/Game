using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Prologue
{
    class SpriteSheet
    {
        public Texture2D SpritesheetImg { get; private set; }
        public int Row { get; private set; }
        public int Frames { get; private set; }
        public Tuple<int,int> Size { get; private set; }
        public bool Animation { get; private set; }
        public string Name { get; private set; }
        public int FrameSize { get; private set; }

        public static List<SpriteSheet> LoadedSpriteSheets = new List<SpriteSheet>();

        public SpriteSheet(string _name)
        {
            this.Name = _name;

            loadSpriteSheetData();
        }

        public void loadSpriteSheetData()
        {
            var Json = File.ReadAllText("C:/Users/david/source/repos/Prologue/Prologue/" + this.Name + ".json");

            try
            {
                var jObject = JObject.Parse(Json);
                if (jObject != null)
                {

                    this.SpritesheetImg = (Texture2D)Game1.prologueContent.GetType().GetProperty(jObject["Image"].ToString()).GetValue(Game1.prologueContent);
                    this.Row = (int)jObject["Row"];
                    this.Frames = (int)jObject["Frames"];
                    this.Size = Tuple.Create((int)jObject["size"]["w"], (int)jObject["size"]["h"]); 
                    this.Animation = (bool)jObject["Animation"];

                    this.FrameSize = this.Size.Item1 / this.Frames;


                }
            }
            catch (Exception)
            {
                Console.WriteLine("Exception Catch -- loadSpriteSheetData, Maybe a wrong name?");
            }
        }

        public static void LoadSpriteSheet(string _spriteSheetName)
        {
            LoadedSpriteSheets.Add(new SpriteSheet(_spriteSheetName));
        }

        public static void UnloadSpriteSheet(string _spriteSheetName)
        {
            LoadedSpriteSheets.Remove(LoadedSpriteSheets.Find(x => x.Name == _spriteSheetName));
        }

        public static void screenDraw(string _spriteSheet, int  Row, int Frame, Vector2 Location, SpriteBatch _spriteBatch,int _size)
        {
            SpriteSheet spriteSheet = LoadedSpriteSheets.Find(x => x.Name == _spriteSheet);
            if (spriteSheet == null)
            {
                Console.WriteLine("SpriteSheet not loaded");
                return;
            }

            Rectangle source = new Rectangle(Frame * spriteSheet.FrameSize,0, spriteSheet.FrameSize, spriteSheet.FrameSize);

            //Console.WriteLine();

            _spriteBatch.Draw(spriteSheet.SpritesheetImg, new Rectangle((int)Location.X, (int)Location.Y,_size,_size), source, Color.White);
            //_spriteBatch.Draw(Game1.prologueContent.imgPlayer, new Rectangle((int)Location.X, (int)Location.Y, (int)Screen.GridSize, (int)Screen.GridSize), Color.White);
        }
    }
}
