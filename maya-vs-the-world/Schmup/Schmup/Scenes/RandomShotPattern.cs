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
        private static Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }



        public RandomShotPattern(LuxGame game, uint shotNb, Vector2 direction, uint angleBtwShotsDegrees, Texture2D bulletText, int deviation)
            : base(game, shotNb, direction, angleBtwShotsDegrees, bulletText)
        {
            int randomNumber = RandomNumber(-deviation, deviation);
            randomDeviation = 2 * (float)Math.PI * randomNumber / 360;
            this.Direction(Vector2.Transform(direction, Matrix.CreateRotationZ(deviation)));
        }
    }
}
