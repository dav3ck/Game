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
        private dynamic FullEvent { get; set; }
        private dynamic CurrentAction { get; set; }
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
                        this.FullEvent = item["Order"];
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
                if (this.FullEvent.Count - 1 == CurrentStep)
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
            this.CurrentAction = this.FullEvent[CurrentStep];
            Console.WriteLine(CurrentAction);
            WaitToFinish = this.CurrentAction["Wait"];
            this.CurrentEventType = CurrentAction["Type"];

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
            string _name = this.CurrentAction["Speaker"];
            string _text = this.CurrentAction["Text"];



            Textbox.TextBoxes.Add(new SpeechTextbox(_text,"Event", _name));
        }

        private void Question()
        {

            string _speaker = this.CurrentAction["Speaker"];
            string _text = this.CurrentAction["Text"];

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
            string _NPC = this.CurrentAction["NPCname"];
            int _XLocation = this.CurrentAction["Location"]["X"];
            int _YLocation = this.CurrentAction["Location"]["Y"];

            NPC.NPClist.Add(new NPC(_XLocation, _YLocation, _NPC, Game1.FrontSpriteBatch, Game1.prologueContent));
        }

        private void CreateEventZone()
        {
            int _EventZoneID = this.CurrentAction["EventZone"];

            EventZone.EventZoneList.Add(new EventZone(_EventZoneID));
        }

        private void DeleteEventZone()
        {
            int _EventZoneID = this.CurrentAction["EventZone"];
            
            EventZone.DeleteEventZone(_EventZoneID);
        }

        private void RemoveNPC()
        {
            string _Name = this.CurrentAction["NPCname"];

            NPC.RemoveNPC(_Name);
        }

        private void WalkNPC()
        {
            string _Name = this.CurrentAction["NPCname"];
            int _x = this.CurrentAction["Goal"]["X"];
            int _y = this.CurrentAction["Goal"]["Y"];
            bool _wait = this.CurrentAction["Wait"];

            //NPC.NPCwalk(Tuple.Create(_x,_y ), _Name,_wait);
            var npc = NPC.NPClist.Find(x => x.Name == _Name);
            npc.InitializeWalk(Tuple.Create(_x,_y),_wait);
        }

        private void WalkPlayer()
        {
            int _x = this.CurrentAction["Goal"]["X"];
            int _y = this.CurrentAction["Goal"]["Y"];
            bool _wait = this.CurrentAction["Wait"];

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
