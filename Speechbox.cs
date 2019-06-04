using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Prologue
{
    class SpeechTextbox : Textbox
    {
        public string Name { get; set; }

        public static List<SpeechTextbox> SpeechTextBoxes = new List<SpeechTextbox>();

        public SpeechTextbox(String FullText, string Caller, string _Name) : base(FullText, Caller)
        {
            SpeechTextBoxes.Add(this);

            Name = _Name;

            this.Background = new TextboxBackground(1, true);
            this.Background.SetSize(Lines_Per_Box * yPerLine + yPerLine);
        }

        public void SpeechBoxUpdate()
        {
            TextboxUpdate();
        }

        public void SpeechBoxDraw()
        {
            TextboxDraw();

            Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20, this.Name, new Vector2(Background.LocationNamebox.X + yPerLine / 4, Background.LocationNamebox.Y - TextboxBackground.NameboxSize.Item2 + yPerLine / 2), Color.Black);
        }
    }
}
