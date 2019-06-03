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
    class Textbox
    {
        public Vector2 Location { get; set; }
        public string FullText { get; set; }
        public string OnScreenText { get; set; }

        public int CharPerLine = 50;

        private List<string> Lines { get; set; }
        private List<string> AllLines { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }

        private List<string> LinesOnscreen { get; set; }
        private int OnPage { get; set; }
        public int OnLine { get; set; }
        private int OnLetter { get; set; }
        public bool SkipText { get; set; }
        public bool Continue { get; set; }
        public bool Finish { get; set; }

        private Texture2D TextBoximg { get; set; }
        private SpriteBatch FrontSpriteBatch;
        private PrologueContent prologueContent { get; set; }

        public Rectangle Hitbox { get; set; }

        private int _Timer { get; set; }
        public int Textbox_Speed { get; set; }
        public int Lines_Per_Box { get; set; }

        public string Caller { get; set; }

        public static List<Textbox> TextBoxes = new List<Textbox>();

        public Textbox(String FullText, string Caller)
        {

            Player.Frozen = true;

            this.Caller = Caller;
            this.FullText = FullText;
            this.prologueContent = Game1.prologueContent;

            LoadTextBoxData(1);

            this.Location = new Vector2(Screen.ScreenWidth / 2 - this.SizeX / 2, Screen.ScreenHeight / 2 + 130);

            this.FrontSpriteBatch = Game1.FrontSpriteBatch;
            this.TextBoximg = prologueContent.Tile1;

            LinesOnscreen = new List<string> { "", "", "", "" };
            OnPage = 0;
            OnLine = 0;
            OnLetter = 0;
            SkipText = false;
            Continue = true;
            Finish = false;
            Hitbox = new Rectangle((int)this.Location.X, (int)this.Location.Y, this.SizeX, this.SizeY);

            _Timer = 0;

            //TextBoxes.Add(this);

            //Console.WriteLine("???????????");

            SplitInLines();
        }

        public void LoadTextBoxData(int ID)
        {
            var json = File.ReadAllText("C:/Users/david/source/repos/Prologue/Prologue/Textbox.json");
            try
            {
                var jObject = JObject.Parse(json);
                if (jObject != null)
                {
                    this.Textbox_Speed = (int)jObject["Textbox_Speed"];
                    this.CharPerLine = (int)jObject["Char_Per_Line"];

                    var Size = jObject["Size"];
                    this.SizeX = (int)Size["X_Size"];
                    this.SizeY = (int)Size["Y_Size"];

                    JArray TextboxArray = (JArray)jObject["Types"];
                    if (TextboxArray != null)
                    {
                        foreach (var item in TextboxArray)
                        {
                            if (ID == (int)item["ID"])
                            {
                                this.TextBoximg = (Texture2D)prologueContent.GetType().GetProperty(item["Texture"].ToString()).GetValue(prologueContent);
                                this.Lines_Per_Box = (int)item["Lines_Per_Box"];
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void SplitInLines()
        {
            //Here the Text is first split up in lines where ever the programmer put a @, But if the text is stil longer then the MaxChar that fits in the width of the textbox
            // It is split down even further

            Lines = FullText.Split((char)'@').ToList();
            AllLines = new List<string> { };

            foreach (string _Line in Lines)
            {
                string Templine =  "" ;
                Templine = _Line;

                if (_Line.Count() > CharPerLine)
                {
                    int _AmountOfLines = _Line.Count() / CharPerLine;
                    int x = 0;
                    int CurrentIndex = CharPerLine;
                    while (_AmountOfLines > 0)
                    {
                        if (_Line.ElementAtOrDefault(CurrentIndex) == ' ')
                        {
                            StringBuilder sb = new StringBuilder(Templine);
                            sb[CurrentIndex] = '@';
                            Templine = sb.ToString();
                            _AmountOfLines -= 1;

                            CurrentIndex += CharPerLine;
                        }
                        else
                        {
                            x++;
                            CurrentIndex -= 1;
                        }
                    }
                    AllLines.AddRange(Templine.Split((char)'@').ToList());
                }
                else
                {
                    AllLines.Add(_Line);
                }
            }

            foreach(string y in AllLines)
            {
                Console.WriteLine(y);
            }

       
            FullText = FullText.Replace("@", System.Environment.NewLine);

        }

        public void TextboxUpdate()
        {

            //General Logic for properly displaying all Text Lines Combined with a Slow mode, so the text isnt instant

            if (this.AllLines.Count == OnLine + OnPage * this.Lines_Per_Box)
            {
                Finish = true; //If All the Lines have been displayed
            }
            if (Finish == false && (_Timer % this.Textbox_Speed == 0 || SkipText == true))
            {
                this._Timer = 0;
                if (LinesOnscreen[OnLine].Length == this.AllLines[OnLine + OnPage * this.Lines_Per_Box].Length)
                {
                    if (OnLine == (this.Lines_Per_Box - 1)) // HIIIIIEERRR
                        Continue = false;
                    else { OnLine += 1; OnLetter = 0; }

                }
                else if (Continue == true)
                {
                    LinesOnscreen[OnLine] = LinesOnscreen[OnLine] + (AllLines[OnLine + OnPage * this.Lines_Per_Box][OnLetter]);
                    OnLetter += 1;
                }
            }

            _Timer += 1;

        }

        public void NextPage()
        {
            //If the OnLine variable = 4, Aka the page is full, the game wil wait for PlayerInput to go to the next page, this Page switching happens here.
            Textbox textbox = this;

            textbox.Continue = true;
            textbox.OnLine = 0;
            textbox.OnPage += 1;
            textbox.OnLetter = 0;
            textbox.SkipText = false;

            textbox.LinesOnscreen = new List<string> { "","","",""};
        }

        public void TextboxDraw()
        {
            FrontSpriteBatch.Draw(TextBoximg, new Rectangle((int)(this.Location.X), (int)(this.Location.Y), SizeX, SizeY), Color.Green); // REPROGRAMM

            int y = 440; //Location still hardcoded??? -----------------------------
            for (int x = 0; x < Lines_Per_Box; x++)
            {
                FrontSpriteBatch.DrawString(prologueContent.Arial20, LinesOnscreen[x], new Vector2(220, y), Color.Black);
                y += 40;
            }
        }

        public static void Delete()
        {
            List<Textbox> textbox = Textbox.TextBoxes.FindAll(x => x.Finish == true);

            if (textbox == null) { return; }

            foreach (var x in textbox)
            {
                if (x.Caller == "Event")
                {
                    EventHandler.Continue = true;
                    EventHandler.EventUpdate();
                }
                else
                {
                    Player.Frozen = false;
                }
            }

            SpeechTextbox.SpeechTextBoxes.RemoveAll(x => x.Finish == true);
            InformationTextBox.InformationTextBoxes.RemoveAll(x => x.Finish == true);
            QuestionBox.QuestionBoxes.RemoveAll(x => x.Finish == true);
            Textbox.TextBoxes.RemoveAll(x => x.Finish == true);  

        }

        public static void Update()
        {
            if(SpeechTextbox.SpeechTextBoxes.Any())
            {
                SpeechTextbox.SpeechTextBoxes.ForEach(x => x.SpeechBoxUpdate());
            }

            if (InformationTextBox.InformationTextBoxes.Any())
            {
                InformationTextBox.InformationTextBoxes.ForEach(x => x.InformationBoxUpdate());
            }

            if (QuestionBox.QuestionBoxes.Any())
            {
                QuestionBox.QuestionBoxes.ForEach(x => x.QuestionBoxUpdate());
            }
        }

        public static void Draw()
        {
            if (SpeechTextbox.SpeechTextBoxes.Any())
            {
                SpeechTextbox.SpeechTextBoxes.ForEach(x => x.SpeechBoxDraw());
            }

            if (InformationTextBox.InformationTextBoxes.Any())
            {
                InformationTextBox.InformationTextBoxes.ForEach(x => x.InformationBoxDraw());
            }

            if (QuestionBox.QuestionBoxes.Any())
            {
                QuestionBox.QuestionBoxes.ForEach(x => x.QuestionBoxDraw());
            }
        }
    }


    //Same as with the objects, Different Kind of TextBox wil have different Functions, so a different class.

    class SpeechTextbox : Textbox
    {
        public string Name { get; set; }

        public static List<SpeechTextbox> SpeechTextBoxes = new List<SpeechTextbox>();

        public SpeechTextbox(String FullText, string Caller, string _Name) : base(FullText, Caller)
        {
            SpeechTextBoxes.Add(this);
        }

        public void SpeechBoxUpdate()
        {
            TextboxUpdate();
        }

        public void SpeechBoxDraw()
        {
            TextboxDraw();
        }
    }


    class InformationTextBox : Textbox
    {

        public static List<InformationTextBox> InformationTextBoxes = new List<InformationTextBox>();

        public InformationTextBox(string FullText, string Caller) : base(FullText, Caller)
        {
            InformationTextBoxes.Add(this);
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

    class QuestionBox : Textbox
    {
        public static List<QuestionBox> QuestionBoxes = new List<QuestionBox>();
        public List<string> Answers { get; set; }

        public int SelectedAnswer { get; set; }
        public int AnswerCount { get; set; }

        public QuestionBox(string FullText, string Caller, List<string> _answers) : base(FullText, Caller)
        {

            this.Answers = _answers;

            this.AnswerCount = this.Answers.Count;
            this.SelectedAnswer = 0;

            QuestionBoxes.Add(this);

            /*
            foreach (string answer in _answers)
            {
                Answers.Add(answer.Replace("@", System.Environment.NewLine));
            } */
            QuestionBoxUpdate();
        }

        public void QuestionBoxUpdate()
        {
            TextboxUpdate();

            if (this.Finish == true)
            {

                if (Game1.newKeyboardState.IsKeyDown(Keys.W) && !Game1.oldKeyboardState.IsKeyDown(Keys.W))
                {
                    this.SelectedAnswer -= 1;
                }
                if (Game1.newKeyboardState.IsKeyDown(Keys.S) && !Game1.oldKeyboardState.IsKeyDown(Keys.S))
                {
                    this.SelectedAnswer += 1;

                }
                if(Game1.newKeyboardState.IsKeyDown(Keys.Space) && !Game1.oldKeyboardState.IsKeyDown(Keys.Space))
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

        public void FinishSelection()
        {
            //Store hier de DATA !!
            Console.WriteLine("U done now!");
            //this.Finish = true;
        }

        public void QuestionBoxDraw()
        {
            TextboxDraw();

            if (this.Finish == true)
            {
                int StartPosition = this.OnLine * 40 + 440;

                for (int x = 0; x < this.Answers.Count; x++)
                {
                    if (x == this.SelectedAnswer)
                    {
                        Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20, " X ", new Vector2(220, StartPosition), Color.Black);
                        Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20, Answers[x], new Vector2(230, StartPosition), Color.Black);
                    }
                    else
                    {
                        Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20, Answers[x], new Vector2(230, StartPosition), Color.Black);
                    }
                    StartPosition += 40;
                }
            }
        }
    }

}
