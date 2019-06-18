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
    class EventHandler
    {
        private JArray FullEvent { get; set; }
        private JObject CurrentAction { get; set; }
        public int AmountSteps { get; set; }
        public int CurrentStep { get; set; }
        public int CurrentEventType { get; set; }
        public static bool Continue { get; set; }

        public static List<EventHandler> EventList = new List<EventHandler>();
        public static bool EventRunning {get;set;}
        private static bool WaitToFinish { get; set; }

        public EventHandler(int Event)
        {
            LoadEventData(Event);
            CurrentStep = 0;
            EventRunning = true;
            Continue = false;
            EventList.Add(this);
            LoadAction();
        }

        public void LoadEventData(int Event)
        {
            var Json = File.ReadAllText("C:/Users/david/source/repos/Prologue/Prologue/Events.json");


            try
            {
                var jObject = JObject.Parse(Json);
                if (jObject == null) { return; }

                JArray EventArray = (JArray)jObject["Events"];
                if (EventArray == null) { return; }

                foreach (var item in EventArray)
                {
                    if (Event == (int)item["Event"])
                    {
                        this.FullEvent = (JArray)item["Order"];
                        this.AmountSteps = (int)item["AmountofItems"];
                        Player.FreezePlayer((bool)item["FreezePlayer"]);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void Update()
        {
            if (Continue == true)
            {
                if (this.FullEvent.Count() - 1 == CurrentStep)
                {
                    EventRunning = false;
                    Player.FreezePlayer(false);
                }
                else
                {
                    CurrentStep++;
                    Continue = false;
                    LoadAction();
                }
            }
        }

        private void LoadAction()
        {
            this.CurrentAction = (JObject)this.FullEvent[CurrentStep];
            //Console.WriteLine(CurrentAction);
            WaitToFinish = (bool)this.CurrentAction["Wait"];
            this.CurrentEventType = (int)CurrentAction["Type"];
            //this.CurrentEventType = 4;

            switch (this.CurrentEventType)
            {
                case 1: //SpeechTextbox
                    SpeakAction();
                    break;
                case 2: //Thought Textbox
                    Informationbox();
                    break;
                case 3: // Question Textbox
                    Question();
                    break;
                case 4: // SpawnNPC
                    LoadNPC();
                    break;
                case 5: // RemoveNPC
                    RemoveNPC();
                    break;
                case 6: // Player PlayAnimation
                    break;
                case 7: // NPC PlayAnimation
                    break;
                case 8: // Create EventZone
                    CreateEventZone();
                    break;
                case 9: // Remove EventZone
                    DeleteEventZone();
                    break;
                case 10: // Player Walk
                    WalkPlayer();
                    break;
                case 11: // NPC walk
                    WalkNPC();
                    break;
                case 12: // Move Camera
                    MoveCamera();
                    break;
                case 13: // Remove Objects
                    RemoveObj();
                    break;
                case 14: // Create Objects
                    CreateObj();
                    break;
                case 15: //Lock cam back on player
                    LockCamPlayer();
                    break;
                case 16: // Enable/Disable Autofollow
                    AutoFollow();
                    break;
                case 17: // Freezes Player
                    Player.FreezePlayer(true);
                    break;
                case 18: // UnFreezes Player
                    Player.FreezePlayer(false);
                    break;

                    
            }

            if (WaitToFinish == false)
            {
                Continue = true;
                //Update();
            }
        }


// ---------------------------------------------------------------------


        private void SpeakAction()
        {
            string _name = (string)this.CurrentAction["Speaker"];
            string _text = (string)this.CurrentAction["Text"];



            Textbox.TextBoxes.Add(new SpeechTextbox(_text,"Event", _name));
        }

        private void Question()
        {

            string _speaker = (string)this.CurrentAction["Speaker"];
            string _text = (string)this.CurrentAction["Text"];

            var _answers = this.CurrentAction["Answers"];
            List<string> _answerslist = new List<string>();
            foreach (string x in _answers)
            {
                _answerslist.Add(x);
            }

            Textbox.TextBoxes.Add(new QuestionBox(_text, "Event", _answerslist, _speaker));
        }

        private void Informationbox()
        {
            string _text = (string)this.CurrentAction["Text"];

            Textbox.TextBoxes.Add(new InformationTextBox(_text, "Event"));
        }

        private void LoadNPC()
        {
            string _NPC = (string)this.CurrentAction["NPCname"];
            int _XLocation = (int)this.CurrentAction["Location"]["X"];
            int _YLocation = (int)this.CurrentAction["Location"]["Y"];

            NPC.NPClist.Add(new NPC(_XLocation, _YLocation, _NPC, Game1.FrontSpriteBatch, Game1.prologueContent));
            //NPC.NPClist.Add(new NPC(3, 4, "Mathijs", Game1.FrontSpriteBatch, Game1.prologueContent));
        }

        private void CreateEventZone()
        {
            int _EventZoneID = (int)this.CurrentAction["EventZone"];

            EventZone.EventZoneList.Add(new EventZone(_EventZoneID));
        }

        private void DeleteEventZone()
        {
            int _EventZoneID = (int)this.CurrentAction["EventZone"];
            
            EventZone.DeleteEventZone(_EventZoneID);
        }

        private void RemoveNPC()
        {
            string _Name = (string)this.CurrentAction["NPCname"];

            NPC.RemoveNPC(_Name);
        }

        private void WalkNPC()
        {
            string _Name = (string)this.CurrentAction["NPCname"];
            int _x = (int)this.CurrentAction["Goal"]["X"];
            int _y = (int)this.CurrentAction["Goal"]["Y"];
            bool _wait = (bool)this.CurrentAction["Wait"];

            //NPC.NPCwalk(Tuple.Create(_x,_y ), _Name,_wait);
            var npc = NPC.NPClist.Find(x => x.Name == _Name);
            npc.InitializeWalk(Tuple.Create(_x,_y),_wait);
        }

        private void WalkPlayer()
        {
            int _x = (int)this.CurrentAction["Goal"]["X"];
            int _y = (int)this.CurrentAction["Goal"]["Y"];
            bool _wait = (bool)this.CurrentAction["Wait"];

            Player.PlayerInitializeAutoWalk(Tuple.Create(_x, _y), _wait);
        }

        private void MoveCamera()
        {
            Tuple<int, int> _goal = Tuple.Create((int)this.CurrentAction["Goal"]["x"], (int)this.CurrentAction["Goal"]["y"]);
            int _speed = (int)this.CurrentAction["Speed"];
            bool _return = (bool)this.CurrentAction["Return"];

            Camera.InitializeCinematic(_goal, _speed, _return, "Event");
        }

        private void RemoveObj()
        {
            var _objects = this.CurrentAction["Objects"];

            List<Tuple<int, int>> _TupleList = new List<Tuple<int, int>>();
            foreach (var x in _objects)
            {
                _TupleList.Add(Tuple.Create((int)x[0], (int)x[1]));
            }

            Objects.RemoveObj(_TupleList);
        }

        private void CreateObj()
        {

            var _objects = this.CurrentAction["Objects"];

            List<int[]> _objectList = new List<int[]>();

            foreach(var x in _objects)
            {
                _objectList.Add(new int[] { (int)x[0], (int)x[1], (int)x[2] });
            }
            
            //_objectList.ForEach(x => Console.WriteLine(x[0] + " " + x[1] + " " + x[2]));

            Objects.CreateObj(_objectList);
        }

        private void LockCamPlayer()
        {
            Camera.LockOnPlayer = true;
        }

        private void AutoFollow()
        {
            NPC.ToggleFollow("Mathijs");
        }


        //--------------------------------------------------------------------------------------


        public static void EventUpdate()
        {

            foreach (var _Event in EventHandler.EventList)
            {
                _Event.Update();
            }

            if (EventHandler.EventRunning == false)
            {
                EventHandler.EventList.Clear();
            }
        }


    }
}
