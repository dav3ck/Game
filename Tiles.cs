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
using Newtonsoft.Json.Linq;
using System.IO;

namespace Prologue
{
    class Tiles
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Xcord { get; set; }
        public float Ycord { get; set; }
        public int ID { get; set; }
        public bool Solid { get; set; }
        public string name { get; set; }
        public int chunck { get; set; }

        public Tuple<int,int> TileCords { get; set; }

        public Texture2D imgtile { get; set; }
        public PrologueContent prologueContent { get; set; }
        private SpriteBatch spriteBatch;

        public Tiles(int X, int Y, int ID, float Xcord, float Ycord, Texture2D tileimage, SpriteBatch spriteBatch, PrologueContent prologueContent)
        {
            this.prologueContent = prologueContent;

            GetTileData(ID);

            this.X = X;
            this.Y = Y;

            this.chunck = Chunck.CurrentChunck(this.Y);

            this.Xcord = Xcord;
            this.Ycord = Ycord;
            this.spriteBatch = spriteBatch;

            this.TileCords = Tuple.Create(X, Y);

            if (this.Solid == true)
            {
                SolidTile.AllSolidTiles.Add(new SolidTile(this.TileCords, this.chunck));
            }
        }

        public void Draw()
        {
            spriteBatch.Draw(imgtile,new Rectangle((int)(this.Xcord - Screen.CameraX),(int)(this.Ycord - Screen.CameraY),(int)Math.Ceiling(Screen.GridSize),(int)Math.Ceiling(Screen.GridSize)), Color.White);
        }

        private void GetTileData (int ID)
        {
            var json = File.ReadAllText("C:/Users/david/source/repos/Prologue/Prologue/Tiles.Json");
            try
            {
                var jObject = JObject.Parse(json);
                if (jObject != null)
                {
                    JArray TileArray = (JArray)jObject["Tiles"];
                    if (TileArray != null)
                    {
                        foreach (var tile in TileArray)
                        {
                            if (ID == (int)tile["ID"])
                            {
                                this.ID = (int)tile["ID"];
                                this.Solid = (bool)tile["Solid"];
                                this.name = tile["Name"].ToString();
                                this.imgtile = (Texture2D)prologueContent.GetType().GetProperty(tile["Texture"].ToString()).GetValue(prologueContent);
                                
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
