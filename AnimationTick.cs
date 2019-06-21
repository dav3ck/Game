using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prologue
{
    class AnimationTick
    {
        private string SpriteSheetName { get; set; }
        public int Row { get; set; }
        private int Tick { get; set; }
        private List<int> Itterations = new List<int>();

        public static int Count = 01;
        public static Random x = new Random();
        private bool Synchronize = false;

        public static List<AnimationTick> AnimationTickList = new List<AnimationTick>();

        public AnimationTick(string _spriteSheetName, int _row, SpriteSheet _spriteSheet)
        {
            int _Tick = 0;
            int _FrameCount = 0;

            _spriteSheet.LoadRowData(_spriteSheetName,_row, ref _FrameCount, ref _Tick);

            SpriteSheetName = _spriteSheetName;
            Row = _row;
            Tick = _Tick;
            for(int x = 0; x < _FrameCount; x++)
            {
                Itterations.Add(x);
            }

            AnimationTickList.Add(this);
        }

        public void Update()
        {
            if(AnimationTick.Count % Tick == 0)
            {
                for(int i = 0; i < Itterations.Count(); i++)
                {
                    Itterations[i] += 1;
                    if (Itterations[i] >= Itterations.Count())
                    {
                        Itterations[i] = 0;
                    }
                }
            }
        }

        public int AssignAnimationIndex()
        {
           return x.Next(Itterations.Count());
        }

        public int GetIterationIndexValue(int Index)
        {
            if (Synchronize)
            {
                return Itterations.First();
            }
            return Itterations[Index];
        }

        public static AnimationTick GetAnimationTick(string _name, int _row)
        {
            AnimationTick animationTick = AnimationTickList.Find(x => x.SpriteSheetName == _name && x.Row == _row);
            if (animationTick == null)
            {
                return AnimationTickList.First();
            }
            return animationTick;
        }

        public static void TickUpdate()
        {
            Count++;
            foreach(AnimationTick x in AnimationTickList)
            {
                x.Update();
            }
        }
    }
}
