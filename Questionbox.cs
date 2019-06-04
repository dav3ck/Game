using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Prologue
{
    class QuestionBox : Textbox
    {
        public static List<QuestionBox> QuestionBoxes = new List<QuestionBox>();
        public List<string> Answers { get; set; }

        public int SelectedAnswer { get; set; }
        public int AnswerCount { get; set; }
        public int DisplaySize { get; set; }
        private List<List<string>> SplitAnswers = new List<List<string>>();
        private bool SetDisplaySize { get; set; }
        public string Name { get; set; }
        public bool HasSpeaker { get; set; }

        public QuestionBox(string FullText, string Caller, List<string> _answers, string _name) : base(FullText, Caller)
        {

            this.Name = _name;

            if (Name != "null")
            {
                HasSpeaker = true;
            }

            this.Answers = _answers;

            foreach(string x in Answers)
            {
                SplitAnswers.Add(SplitInLines(x));
            }


            this.AnswerCount = this.Answers.Count;
            this.SelectedAnswer = 0;

            QuestionBoxes.Add(this);

            this.DisplaySize = Lines_Per_Box;

            this.Background = new TextboxBackground(1, HasSpeaker);
            this.Background.SetSize(Lines_Per_Box * yPerLine + yPerLine);

            QuestionBoxUpdate();
        }

        public void QuestionBoxUpdate()
        {
            TextboxUpdate();

            if (this.OnPage + 1 == Math.Ceiling((double)this.AllLines.Count / Lines_Per_Box) && this.SetDisplaySize == false)
            {
                this.SetDisplaySize = true;
                this.DisplaySize = this.OnLine + 1 + (SplitAnswers.Sum(x => x.Count));
                this.Background.SetSize(((AllLines.Count - OnPage * Lines_Per_Box) + 1 + DisplaySize) * yPerLine + yPerLine);
            }

            if (this.Complete == true)
            {

                if (Game1.newKeyboardState.IsKeyDown(Keys.W) && !Game1.oldKeyboardState.IsKeyDown(Keys.W))
                {
                    this.SelectedAnswer -= 1;
                }
                if (Game1.newKeyboardState.IsKeyDown(Keys.S) && !Game1.oldKeyboardState.IsKeyDown(Keys.S))
                {
                    this.SelectedAnswer += 1;
                }
                if (Game1.newKeyboardState.IsKeyDown(Keys.Space) && !Game1.oldKeyboardState.IsKeyDown(Keys.Space))
                {
                    FinishSelection();
                }

                if (SelectedAnswer < 0)
                {
                    this.SelectedAnswer = this.AnswerCount - 1;
                }
                if (SelectedAnswer >= this.AnswerCount)
                {
                    this.SelectedAnswer = 0;
                }
            }
        }

        private int CalculateDisplaySize()
        {
            int QuestionLines = (this.AllLines.Count % 4);
            return (QuestionLines - this.AnswerCount);
        }

        public void FinishSelection()
        {
            //Store hier de DATA !!
            Console.WriteLine("U done now!");
        }

        public void QuestionBoxDraw()
        {
            TextboxDraw();

            if (HasSpeaker == true)
            {
                Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20, this.Name, new Vector2(Background.LocationNamebox.X + yPerLine / 4, Background.LocationNamebox.Y - TextboxBackground.NameboxSize.Item2 + yPerLine / 2), Color.Black);
            }

            if (this.Complete == true)
            {
                int StartPosition = (this.OnLine + 1 ) * yPerLine + (int)Background.LocationBottom.Y - Background.Ysize;

                for (int x = 0; x < this.Answers.Count; x++)
                {

                    List<string> Temp = this.SplitAnswers[x];

                    foreach (string q in Temp) {
                        if (q == Temp[0])
                        {
                            if (x == this.SelectedAnswer)
                            {
                                Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20, " X ", new Vector2(Background.LocationBottom.X, StartPosition), Color.Black);
                                Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20, q, new Vector2(Background.LocationBottom.X + yPerLine / 2, StartPosition), Color.Black);
                            }
                            Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20, q, new Vector2(Background.LocationBottom.X + yPerLine / 2, StartPosition), Color.Black);
                        }
                        else
                        {
                            Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20, q, new Vector2(Background.LocationBottom.X + yPerLine, StartPosition), Color.Black);
                        }
                        StartPosition += 40;
                    }

                }
            }
        }
    }
}
