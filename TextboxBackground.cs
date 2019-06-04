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
    class TextboxBackground
    {
        private static Texture2D BackupImg = Game1.prologueContent.Tile1;
        public static int StartingHeight { get; set; }
        public static JArray BackgroundTypes { get; set; }

        public Texture2D BackgroundImg { get; private set; }

        public Vector2 LocationBottom { get; private set; }
        public int Xsize { get; private set; }
        public int Ysize { get; private set; }
        public bool Name { get; set; }

        public  Vector2 LocationNamebox { get; private set; }
        public static float NameboxXcord { get; set; }
        public static Tuple<int,int> NameboxSize { get; set; }

        private Vector2 NameLocation {get;set;}

        public TextboxBackground(int Type, bool _name)
        {

            ChangeType(Type);

            Name = _name;
        }

        public void ChangeType(int Type) 
        {
            try
            {
                if (BackgroundTypes != null)
                {
                    foreach (var item in BackgroundTypes)
                    {
                        if ((int)item["ID"] == Type)
                        {
                            BackgroundImg = (Texture2D)Game1.prologueContent.GetType().GetProperty(item["Image"].ToString()).GetValue(Game1.prologueContent);
                            Xsize = (int)item["Size"]["w"];
                            Ysize = (int)item["Size"]["h"];

                            CalculateLocation();
                        }
                    }
                }
            }
            catch (Exception){
                Console.WriteLine("Error Bij change van Types");
                if (Type != 1)
                {
                    ChangeType(1);
                }
                else { Console.WriteLine("RIPPPPP"); }
            }
        }

        public void FindType()
        {
            // MISCH HIER WAT LEUKS DOEN???
        }

        public void SetSize(int _y)
        {
            Ysize = _y;
            CalculateLocation();
        }

        private void CalculateLocation()
        {
            LocationBottom = new Vector2(Screen.ScreenWidth / 2 - this.Xsize / 2, StartingHeight);
            LocationNamebox = new Vector2(LocationBottom.X + this.Xsize * NameboxXcord, LocationBottom.Y - Ysize);
        }

        public void Draw()
        {
            Game1.FrontSpriteBatch.Draw(this.BackgroundImg, new Rectangle((int)LocationBottom.X,(int)LocationBottom.Y - Ysize,Xsize,Ysize) , Color.Green);

            if (Name == true)
            {
                Game1.FrontSpriteBatch.Draw(this.BackgroundImg, new Rectangle((int)LocationNamebox.X, (int)LocationNamebox.Y - NameboxSize.Item2, NameboxSize.Item1, NameboxSize.Item2), Color.Blue);
            }
        }



    }
}
