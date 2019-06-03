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

        private static List<string> LinesOnscreen { get; set; }
        private static int OnPage { get; set; }
        private static int OnLine { get; set; }
        private static int OnLetter { get; set; }
        public static bool SkipText { get; set; }
        public static bool Continue { get; set; }
        public static bool Finish { get; set; }

        private Texture2D TextBoximg { get; set; }
        private SpriteBatch FrontSpriteBatch;
        private PrologueContent prologueContent { get; set; }

        public Rectangle Hitbox { get; set; }

        private int _Timer { get; set; }
        public int Textbox_Speed { get; set; }
        public int Lines_Per_Box { get; set; }

        public static string Caller { get; set; }

        public static List<Textbox> TextBoxes = new List<Textbox>();

        public Textbox(String FullText, string Caller)
        {
            Delete();

            Player.Frozen = true;

            Textbox.Caller = Caller;
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

            TextBoxes.Add(this);

            Console.WriteLine("???????????");

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

        public void TextBoxUpdate()
        {

            //General Logic for properly displaying all Text Lines Combined with a Slow mode, so the text isnt instant

            if (this.AllLines.Count == OnLine + OnPage * this.Lines_Per_Box)
            {
                Finish = true;
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

        public static void NextPage()
        {

            //If the OnLine variable = 4, Aka the page is full, the game wil wait for PlayerInput to go to the next page, this Page switching happens here.
            if (Continue == false)
            {
                Continue = true;
                OnLine = 0;
                OnPage += 1;
                OnLetter = 0;
                SkipText = false;

                LinesOnscreen = new List<string>{ "","","",""};
                    
            }
        }

        public void Draw()
        {
            FrontSpriteBatch.Draw(TextBoximg, new Rectangle((int)(this.Location.X), (int)(this.Location.Y), SizeX, SizeY), Color.Green); // REPROGRAMM

            int y = 440;
            for (int x = 0; x < Lines_Per_Box; x++)
            {
                FrontSpriteBatch.DrawString(prologueContent.Arial20, LinesOnscreen[x], new Vector2(220, y), Color.Black);
                y += 40;
            }
        }

        public static void Delete()
        {
            TextBoxes.Clear();

            if (Textbox.Caller == "Event")
            {
                EventHandler.Continue = true;
                Textbox.Caller = "";
                EventHandler.EventUpdate();
            }
            else
            {
                Player.Frozen = false;
            }
        }
    }


    //Same as with the objects, Different Kind of TextBox wil have different Functions, so a different class.

    class InformationTextBox : Textbox
    {
        public InformationTextBox(String FullText, string Type , int SizeX, int SizeY, SpriteBatch FrontSpriteBatch, PrologueContent prologueContent) : base(FullText, Type)
        {
        }

    }


  /*  class QuestionTextBox : Textbox
    {

    } */
}
