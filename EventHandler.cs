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
            Player.Frozen = true;

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
                    Player.Frozen = false;
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
            }

            if (WaitToFinish == false)
            {
                Continue = true;
                Update();
            }
        }

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
