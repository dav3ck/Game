using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prologue
{
    class InformationTextBox : Textbox
    {

        public static List<InformationTextBox> InformationTextBoxes = new List<InformationTextBox>();

        public InformationTextBox(string FullText, string Caller) : base(FullText, Caller)
        {
            InformationTextBoxes.Add(this);
            this.Background = new TextboxBackground(1, false);
            this.Background.SetSize(Lines_Per_Box * yPerLine + yPerLine);
        }

        public void InformationBoxUpdate()
        {
            TextboxUpdate();
        }

        public void InformationBoxDraw()
        {
            TextboxDraw();
        }
    }
}
