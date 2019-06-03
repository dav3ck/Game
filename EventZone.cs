using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Prologue
{
    class EventZone
    {
        public static List<EventZone> EventZoneList = new List<EventZone>();

        public int EventZoneID { get; set; }
        public int YPosition { get; set; }
        private int LoadEvent { get; set; }
        public bool SingleUse { get; set; }

        public EventZone(int ID)
        {
            this.EventZoneID = ID;
            LoadEventZoneData();
        }

        private void LoadEventZoneData()
        {
            var Json = File.ReadAllText("C:/Users/david/source/repos/Prologue/Prologue/EventZones.json");

            try
            {

                var jObject = JObject.Parse(Json);
                if (jObject == null) { return; }

                JArray EventZoneArray = (JArray)jObject["EventZones"];
                if (EventZoneArray == null) { return; }

                foreach (var _EventZone in EventZoneArray)
                {
                    if ((int)_EventZone["ID"] == this.EventZoneID) 
                    {
                        this.YPosition = (int)_EventZone["YPosition"];
                        this.LoadEvent = (int)_EventZone["LoadEvent"];
                        this.SingleUse = (bool)_EventZone["SingleUse"];
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void DeleteEventZone(int _ID)
        {
            EventZoneList.RemoveAll(x => x.EventZoneID == _ID);
        }

        public void TriggerEventZone()
        {
            Console.WriteLine(this.LoadEvent);
            EventHandler.EventList.Add(new EventHandler(this.LoadEvent));

            if (SingleUse == true)
            {
                DeleteEventZone(this.EventZoneID);
            }
        }

        public static void PlayerIntersection(int _PlayerYtile)
        {
            EventZone Event = EventZoneList.Find(x => x.YPosition == _PlayerYtile);
            if (Event != null)
            {
                Player.Horizontal_Momentum = 0;
                Player.Vertical_Momentum = 0;

                Event.TriggerEventZone();
            }
        }


    }
}
