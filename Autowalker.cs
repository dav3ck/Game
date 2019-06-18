using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prologue
{
    class Autowalker
    {

        private List<Tuple<int,int>> Path { get; set; }
        private string Name { get; set; }
        public static List<Autowalker> AutowalkerList = new List<Autowalker>();

        private int Step { get; set; }
        private Tuple<int,int> CurrentPosition { get; set; }
        private Tuple<int,int> Goal { get; set; }
        private Tuple<int,int> Speed { get; set; }
        private Tuple<float,float> CurrentCords { get; set; }

        private float X_towalk { get; set; }
        private float Y_towalk { get; set; }

        /*private int MomentumX { get; set; }
        private int MomentumY { get; set; } */

        private Tuple<int,int> Momentum { get; set; }

        private bool Finished { get; set; }
        private bool EventWait { get; set; }


        public Autowalker(string _name, List<Tuple<int,int>> _path, Tuple<float,float> _currentcords, Tuple<int,int> _speed, bool _eventwait)
        {


            this.Path = _path;
            this.Name = _name;
            this.CurrentCords = _currentcords;
            this.CurrentPosition = Screen.GridCords(CurrentCords.Item1, CurrentCords.Item2);
            Console.WriteLine("StartTiles: " + this.CurrentPosition);
            this.Speed = _speed;
            this.EventWait = _eventwait;

            this.Step = 0;

            this.Goal = Path[Step];

            Autowalker.AutowalkerList.Add(this);

            Console.WriteLine(AutowalkerList.Count);


            //WalkDirection(this.CurrentPosition, Goal);
            CalculateWalk();
        }

        private void CenterinTile()
        {

            Tuple<int,int> NextPosition = this.Path[1];
            Console.WriteLine("MOET NAAR DEZE TILE" + this.Path[1]);

            Tuple<int, int> TileCords = Screen.ScreenCords(NextPosition.Item1,NextPosition.Item2);
            TileCords = Tuple.Create((int)(TileCords.Item1 + (Screen.GridSize / 2)), (int)(TileCords.Item2 + (Screen.GridSize / 2)));

            Tuple<int,int> Directions = WalkDirection(Tuple.Create((int)this.CurrentCords.Item1,(int) this.CurrentCords.Item2), Tuple.Create((int)TileCords.Item1, (int)TileCords.Item2));

            this.Momentum = Tuple.Create(Directions.Item1 * this.Speed.Item1, Directions.Item2 * this.Speed.Item2);

            Console.WriteLine("DE TILE CORDS" + TileCords);
            Console.WriteLine("De player Cords" + this.CurrentCords);

            this.X_towalk = Math.Abs(TileCords.Item1 - this.CurrentCords.Item1);
            this.Y_towalk = Math.Abs(TileCords.Item2 - this.CurrentCords.Item2);


        }

        private Tuple<int,int> WalkDirection(Tuple<int,int> Cord1, Tuple<int,int> Cord2)
        {
            int DirectionX = Cord2.Item1 - Cord1.Item1;
            int DirectionY = Cord2.Item2 - Cord1.Item2;

            int _DirectionX = 0;
            int _DirectionY = 0;

            if (DirectionX < 0)
            {
                _DirectionX = -1;
            }
            else if (DirectionX > 0)
            {
                _DirectionX = 1;
            }
            else { _DirectionX = 0; }

            if (DirectionY < 0)
            {
                _DirectionY = -1;
            }
            else if (DirectionY > 0)
            {
                _DirectionY = 1;
            }
            else { _DirectionY = 0; }

            return Tuple.Create(_DirectionX, _DirectionY);
        }

        private void CalculateWalk()
        {
            Tuple<int, int> Directions = WalkDirection(this.CurrentPosition, this.Goal);
            Tuple<int, int> GoalCords = Screen.ScreenCords(this.Goal.Item1, this.Goal.Item2);
            Tuple<int, int> EntityCords = Tuple.Create((int)GoalCords.Item1, (int)GoalCords.Item2);

            if (this.Name == "Player")
            {
                EntityCords = Player.GetPlayerCords(Player.Player1);
            }
            else
            {
                try
                {
                    EntityCords = NPC.NPClist.Find(x => x.Name == this.Name).GetNPCCords();
                }
                catch (Exception)
                {
                    Console.WriteLine("Filled in Name in CalculatePath() Function failed in an attempt to get NPC coordinates");
                }
            }

            if (Directions.Item1 != 0) { this.X_towalk = Math.Abs(GoalCords.Item1 + (Screen.GridSize / 2) - EntityCords.Item1); }
            if (Directions.Item2 != 0) { this.Y_towalk = Math.Abs(GoalCords.Item2 + (Screen.GridSize / 2) - EntityCords.Item2); }

            this.Momentum = Tuple.Create(Directions.Item1 * this.Speed.Item1, Directions.Item2 * this.Speed.Item2);
        }

        public void Update()
        {

            if (this.X_towalk <= 0 && this.Y_towalk <= 0)
            {
                this.Momentum = Tuple.Create(0, 0);

                this.CurrentPosition = this.Goal;

                this.Step += 1;

                if (!(this.Step >= Path.Count()))
                {
                    this.Goal = Path[Step];
                    CalculateWalk();
                }
                else
                {
                    this.Finished = true;
                    if (this.EventWait == true) { EventHandler.Continue = true; }
                }
            }
            else
            {
                this.X_towalk -= Math.Abs(this.Momentum.Item1);
                this.Y_towalk -= Math.Abs(this.Momentum.Item2);

                if (this.Name == "Player")
                {
                    Player.MovePlayer(this.Momentum);
                }
                else
                {
                    NPC.WalkNPC(this.Name, this.Momentum);
                }
            }
        }

        public static void DeleteAutoWalker()
        {
            Autowalker.AutowalkerList.RemoveAll(x => x.Finished == true);
        }

        public static void ManualDeleteAutoWalker(Autowalker x)
        {
            try
            {
                Autowalker.AutowalkerList.Remove(x);
            }
            catch (Exception){
                Console.WriteLine("GAAT FOUT");
            }
        }

        public static bool CalculateFaceDirection(Tuple<float,float> Momentum)
        {
            if (Momentum.Item1 != 0 && Momentum.Item2 == 0)
            {
                return true;
            }
            else { return false; }
        }
    }
}
