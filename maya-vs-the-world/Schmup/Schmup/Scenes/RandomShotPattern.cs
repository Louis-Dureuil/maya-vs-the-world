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
        private float randomDeviation;

        public RandomShotPattern(LuxGame game, uint shotNb, Vector2 direction, uint angleBtwShotsDegrees, Texture2D bulletText, int deviation)
            : base(game, shotNb, direction, angleBtwShotsDegrees, bulletText)
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 1000);
            randomDeviation = randomNumber;
            direction = Vector2.Transform(direction, Matrix.CreateRotationZ(deviation));
        }
    }
}
