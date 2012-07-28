using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class RandomShotPattern : ShotPattern
    {
        public RandomShotPattern(LuxGame game, int shotNb, Vector2 direction, int angleBtwShotsDegrees, Texture2D bulletText, int deviationDegrees)
            : base(game, shotNb, direction, angleBtwShotsDegrees, bulletText)
        {
            int randomNumber = Common.Rand.Next(-deviationDegrees, deviationDegrees);
            float randomDeviationRadian = 2 * (float)Math.PI * randomNumber / 360;
            this.Direction=Vector2.Transform(direction, Matrix.CreateRotationZ(randomDeviationRadian));
        }
    }
}
