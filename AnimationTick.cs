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
        public int Itteration { get; set; }
        private int FrameCount { get; set; }

        public static int Count = 0;
        public static Random x = new Random();

        public AnimationTick(string _spriteSheetName, int _row)
        {
            int _Tick = 0;
            int _FrameCount = 0;

            SpriteSheet.LoadRowData(_spriteSheetName, _row, ref _FrameCount, ref _Tick);

            SpriteSheetName = _spriteSheetName;
            Row = _row;
            Tick = _Tick;
            FrameCount = _FrameCount;
            Itteration = x.Next(FrameCount); ;
        }

        public void Update()
        {
            if(AnimationTick.Count % Tick == 0)
            {
                Itteration++;
            }
            if (Itteration > FrameCount)
            {
                Itteration = 0;
            } 
        }

        public static void TickUpdate()
        {
            Count++;
        }
    }
}
