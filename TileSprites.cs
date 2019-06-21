using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Prologue
{
    class TileSprites
    {
        protected AnimationTick Animation { get; set; }
        protected bool IsAnimated { get; set; }
        protected string SpriteSheetName { get; set; }
        protected int AnimationRow { get; set; }
        protected int AnimationIndex { get; set; }

        protected int Xcord { get; set; }
        protected int Ycord { get; set; }
        protected SpriteBatch spriteBatch { get; set; }

        protected void InitializeAnimation(string _SpriteSheetName, int _AnimationRow, int _AnimationIndex)
        {
            SpriteSheetName = _SpriteSheetName;
            AnimationRow = _AnimationRow;
            AnimationIndex = _AnimationIndex;

            IsAnimated = SpriteSheet.IsAnimated(SpriteSheet.GetSpriteSheet(SpriteSheetName));
            if (IsAnimated == true)
            {
                Animation = AnimationTick.GetAnimationTick(SpriteSheetName, 0);
                AnimationIndex = Animation.AssignAnimationIndex();
            }
        }

        public void Draw()
        {
            int x = 1;
            if (IsAnimated)
            {
                x = Animation.GetIterationIndexValue(AnimationIndex);
            }

            SpriteSheet.screenDraw("Test_Animation_SpriteSheet", 0, x, new Vector2((int)(this.Xcord - Screen.CameraX), (int)(this.Ycord - Screen.CameraY)), Game1.FrontSpriteBatch, (int)Screen.GridSize, (int)Screen.GridSize);
        }
    }
}
