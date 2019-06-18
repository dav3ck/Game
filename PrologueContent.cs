using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Prologue
{
    public class PrologueContent
    {
        public Texture2D Tile1 { get; set; }
        public Texture2D Tile2 { get; set; }
        public Texture2D imgPlayer { get; set; } 
        public Texture2D Path_Left_1 { get; set; }
        public SpriteFont Arial20 { get; set; }
        public Texture2D Textbox_Sprite { get; set; }
        public Texture2D Testbox_Sprite { get; set; }
        public Texture2D Player_Walk_SpriteSheet { get; set; }
        public Texture2D Test_Animation_SpriteSheet { get; set; }

        public PrologueContent(ContentManager Content)
        {
            Tile1 = Content.Load<Texture2D>("Path_Side_Right_1");
            Tile2 = Content.Load<Texture2D>("Path_Side_Left_1");
            Path_Left_1 = Content.Load<Texture2D>("Path_Middle_Left_1");
            imgPlayer = Content.Load<Texture2D>("PlayerPlaceHolder");
            Textbox_Sprite = Content.Load<Texture2D>("Textbox_SpriteSheet");
            Testbox_Sprite = Content.Load<Texture2D>("Testbox");
            Player_Walk_SpriteSheet = Content.Load<Texture2D>("Player_Walk");
            Test_Animation_SpriteSheet = Content.Load<Texture2D>("Test_Animation_SpriteSheet");

            Arial20 = Content.Load<SpriteFont>("Arial20");
        }
    }
}
