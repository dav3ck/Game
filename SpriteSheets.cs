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
        public Tuple<int,int> FrameSize { get; private set; }
        public JArray Rows { get; private set; }

        public static List<SpriteSheet> LoadedSpriteSheets = new List<SpriteSheet>();

        public SpriteSheet(string _name)
        {
            this.Name = _name;

            loadSpriteSheetData();

            if (Animation)
            {
                LoadAnimation();
            }
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

                    this.FrameSize = Tuple.Create(this.Size.Item1 / this.Frames, this.Size.Item2 / this.Row);
                    this.Rows = (JArray)jObject["Rows"];
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Exception Catch -- loadSpriteSheetData, Maybe a wrong name? {0}", Name);
            }
        }

        public void LoadRowData(string _name, int _row, ref int FrameCount, ref int ItterationTick)
        {
            try
            {
                JObject Row = (JObject)Rows.ElementAt(_row);
                if (Row == null)
                {
                    return;
                }

                FrameCount = Row["Frames"].Count();
                ItterationTick = (int)Row["Time"];

            }
            catch (Exception)
            {
                Console.WriteLine("GAAT IETS FOUT BIJ LOADROWDATA");
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


        public static void screenDraw(string _spriteSheet, int  Row, int Frame, Vector2 Location, SpriteBatch _spriteBatch,int _sizeX, int _sizeY)
        {
            SpriteSheet spriteSheet = LoadedSpriteSheets.Find(x => x.Name == _spriteSheet);
            if (spriteSheet == null)
            {
                Console.WriteLine("SpriteSheet not loaded");
                return;
            }

            Rectangle source = new Rectangle(Frame * spriteSheet.FrameSize.Item1,Row * spriteSheet.FrameSize.Item2, spriteSheet.FrameSize.Item1, spriteSheet.FrameSize.Item2);

            _spriteBatch.Draw(spriteSheet.SpritesheetImg, new Rectangle((int)Location.X, (int)Location.Y,_sizeX,_sizeY), source, Color.White);
            //_spriteBatch.Draw(Game1.prologueContent.imgPlayer, new Rectangle((int)Location.X, (int)Location.Y, (int)Screen.GridSize, (int)Screen.GridSize), Color.White);
        }

        public static bool IsAnimated(SpriteSheet _spritesheet)
        {
            return _spritesheet.Animation;
        }

        public static SpriteSheet GetSpriteSheet(string _spriteSheetName)
        {
            SpriteSheet spriteSheet = LoadedSpriteSheets.Find(x => x.Name == _spriteSheetName);
            if(spriteSheet == null)
            {
                LoadSpriteSheet(_spriteSheetName);
                return GetSpriteSheet(_spriteSheetName);
            }
            return spriteSheet;
        }

        public void LoadAnimation()
        {
            for(int x = 0; x < Rows.Count(); x++)
            {
                AnimationTick _animation = new AnimationTick(Name, x, this);
            }
        }
    }
}
