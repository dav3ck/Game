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
        public static Vector2 Location { get; set; }
        public string FullText { get; set; }
        public string OnScreenText { get; set; }

        public static int CharPerLine { get; set; }

        private List<string> Lines { get; set; }
        public List<string> AllLines { get; set; }
        public static int SizeX { get; set; }
        public static int SizeY { get; set; }

        public List<string> LinesOnscreen { get; set; }
        public int OnPage { get; set; }
        public int OnLine { get; set; }
        private int OnLetter { get; set; }
        public bool SkipText { get; set; }
        public bool Continue { get; set; }
        public bool Complete { get; set; }
        public bool Finish { get; set; }

        public static int yPerLine {get;set;}

        public TextboxBackground Background { get; set; }

        private static Texture2D TextBoximg = Game1.prologueContent.Tile1;
        private SpriteBatch FrontSpriteBatch;
        private PrologueContent prologueContent { get; set; }

        public Rectangle Hitbox { get; set; }

        private int _Timer { get; set; }
        public static int Textbox_Speed { get; set; }
        public static int Lines_Per_Box { get; set; }

        public string Caller { get; set; }

        public static List<Textbox> TextBoxes = new List<Textbox>();

        public Textbox(String FullText, string Caller)
        {

            Player.FreezePlayer(true);

            this.Caller = Caller;
            this.FullText = FullText;
            this.prologueContent = Game1.prologueContent;

            Location = new Vector2(Screen.ScreenWidth / 2 - SizeX / 2, Screen.ScreenHeight / 2 + 130);

            this.FrontSpriteBatch = Game1.FrontSpriteBatch;

            LinesOnscreen = new List<string> { "", "", "", "" };
            OnPage = 0;
            OnLine = 0;
            OnLetter = 0;
            SkipText = false;
            Continue = true;
            Complete = false;
            Finish = false;
            Hitbox = new Rectangle((int)Location.X, (int)Location.Y, SizeX, SizeY);

            _Timer = 0;

            //TextBoxes.Add(this);

            //Console.WriteLine("???????????");

            this.AllLines = SplitInLines(this.FullText);
        }

        public static void LoadTextBoxData()
        {
            int ID = 1; // ACHTERAF HIER NAAR KIJKEN,

            var json = File.ReadAllText("C:/Users/david/source/repos/Prologue/Prologue/Textbox.json");
            try
            {
                var jObject = JObject.Parse(json);
                if (jObject != null)
                {
                    Textbox_Speed = (int)jObject["Textbox_Speed"];
                    CharPerLine = (int)jObject["Char_Per_Line"];

                    var Size = jObject["Size"];
                    SizeX = (int)Size["X_Size"];
                    SizeY = (int)Size["Y_Size"];
                    yPerLine = (int)jObject["yPerLine"];

                    TextboxBackground.StartingHeight = (int)(Screen.ScreenHeight * (float)jObject["TextboxYcord"]);
                    TextboxBackground.BackgroundTypes = (JArray)jObject["TextboxBackgroundTypes"];
                    TextboxBackground.NameboxSize = Tuple.Create((int)jObject["NameboxSize"]["w"], (int)jObject["NameboxSize"]["h"]);
                    TextboxBackground.NameboxXcord = (float)jObject["NameboxXcord"];

                    JArray TextboxArray = (JArray)jObject["Types"];
                    if (TextboxArray != null)
                    {
                        foreach (var item in TextboxArray)
                        {
                            if (ID == (int)item["ID"])
                            {
                                TextBoximg = (Texture2D)Game1.prologueContent.GetType().GetProperty(item["Texture"].ToString()).GetValue(Game1.prologueContent);
                                Lines_Per_Box = (int)item["Lines_Per_Box"];
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Gaat iets fout bij inladen Van TextboxData!");
            }
        }

        public static List<string> SplitInLines(string _fulltext)
        {
            //Here the Text is first split up in lines where ever the programmer put a @, But if the text is stil longer then the MaxChar that fits in the width of the textbox
            // It is split down even further

            List<string> Lines = _fulltext.Split((char)'@').ToList();
            List<string> AllLines = new List<string> { };

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

            return AllLines;
            //FullText = FullText.Replace("@", System.Environment.NewLine);

        }

        public void TextboxUpdate()
        {
            if (Game1.newKeyboardState.IsKeyDown(Keys.Space) && !Game1.oldKeyboardState.IsKeyDown(Keys.Space))
            {
                if (this.Continue == false)
                {
                    this.SkipText = false;
                    this.NextPage();
                }

                else { this.SkipText = true; }

                if (this.Complete == true)
                {
                    this.Finish = true;
                }
            }

            //General Logic for properly displaying all Text Lines Combined with a Slow mode, so the text isnt instant

            if (this.AllLines.Count == OnLine + OnPage * Lines_Per_Box)
            {
                Complete = true; //If All the Lines have been displayed
            }
            if (Complete == false && (_Timer % Textbox_Speed == 0 || SkipText == true))
            {
                this._Timer = 0;
                if (LinesOnscreen[OnLine].Length == this.AllLines[OnLine + OnPage * Lines_Per_Box].Length)
                {
                    if (OnLine == (Lines_Per_Box - 1)) // HIIIIIEERRR
                        Continue = false;
                    else { OnLine += 1; OnLetter = 0; }

                }
                else if (Continue == true)
                {
                    LinesOnscreen[OnLine] = LinesOnscreen[OnLine] + (AllLines[OnLine + OnPage * Lines_Per_Box][OnLetter]);
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
            //Game1.FrontSpriteBatch.Draw(TextBoximg, new Rectangle((int)(Location.X), (int)(Location.Y), SizeX, SizeY), Color.Green); // REPROGRAMM

            int _x = (int)Background.LocationBottom.X + yPerLine / 4;

            int y = (int)Background.LocationBottom.Y + yPerLine / 2 - Background.Ysize;


            Background.Draw();
            //Location still hardcoded??? -----------------------------
            for (int x = 0; x < Lines_Per_Box; x++)
            {
                Game1.FrontSpriteBatch.DrawString(Game1.prologueContent.Arial20,this.LinesOnscreen[x], new Vector2(_x, y), Color.Black);
                y += yPerLine;
            }         
        }

       /* public static void DrawBox(int _xTiles, int _yTiles, bool _Name, int _StartY, int _yPerLine)
        {
            int _StartX = 220;


            for(int y = 0; y < _yTiles; y++)
            {
                for(int x = 0; x < _xTiles; x++)
                {
                    if(x == 0 && y == 0)
                    {
                        SpriteSheet.screenDraw("Textbox_SpriteSheet", 0, 0, new Vector2(_StartY, _StartX),Game1.FrontSpriteBatch,_yPerLine);
                    }
                    else if( x == 0 && y == _yTiles)
                    {
                        SpriteSheet.screenDraw("Textbox_SpriteSheet", 0, 2, new Vector2(_StartY + _yPerLine * _yTiles, _StartX), Game1.FrontSpriteBatch, _yPerLine);
                    }
                    else if (x == 0)
                    {
                        SpriteSheet.screenDraw("Textbox_SpriteSheet", 0, 1, new Vector2(_StartY + _yPerLine * y, _StartX), Game1.FrontSpriteBatch, _yPerLine);
                    }
                    else if( x == _xTiles && y == 0)
                    {
                        SpriteSheet.screenDraw("Textbox_SpriteSheet", 2, 0, new Vector2(_StartY, _StartX + _yPerLine * _xTiles), Game1.FrontSpriteBatch, _yPerLine);
                    }
                    else if (y == 0)
                    {
                        SpriteSheet.screenDraw("Textbox_SpriteSheet", 1, 0, new Vector2(_StartY, _StartX + _yPerLine * x), Game1.FrontSpriteBatch, _yPerLine);
                    }
                    else if(x == _xTiles && y == _yTiles)
                    {
                        SpriteSheet.screenDraw("Textbox_SpriteSheet", 2, 2, new Vector2(_StartY + _yPerLine * _yTiles, _StartX + _yPerLine * _xTiles), Game1.FrontSpriteBatch, _yPerLine);
                    }
                    else if(x == _xTiles)
                    {
                        SpriteSheet.screenDraw("Textbox_SpriteSheet", 1, 2, new Vector2(_StartY + _yPerLine * y, _StartX + _yPerLine * _xTiles), Game1.FrontSpriteBatch, _yPerLine);
                    }
                    else if (y == _yTiles)
                    {
                        SpriteSheet.screenDraw("Textbox_SpriteSheet", 2, 1, new Vector2(_StartY + _yPerLine * _yTiles, _StartX + _yPerLine * x), Game1.FrontSpriteBatch, _yPerLine);
                    }
                    else
                    {
                        SpriteSheet.screenDraw("Textbox_SpriteSheet", 1, 1, new Vector2(_StartY + _yPerLine * y, _StartX + _yPerLine * x), Game1.FrontSpriteBatch, _yPerLine);
                    }
                }
            }
        } */

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
                    Player.FreezePlayer(false);
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

            if (Textbox.TextBoxes.Any(x=> x.Finish == true))
            {
                Textbox.Delete();
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






}
