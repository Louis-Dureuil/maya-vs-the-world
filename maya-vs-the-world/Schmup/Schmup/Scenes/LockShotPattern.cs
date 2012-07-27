using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class LockShotPattern : ShotPattern
    {

        public LockShotPattern(LuxGame game, int shotNb, int angleBtwShotsDegrees, Texture2D bulletText)
            : base(game, shotNb, new Vector2(0,0), angleBtwShotsDegrees, bulletText)
        {
        }

        public void Shoot(Vector2 direction)
        {
            this.Direction(direction);
            this.Shoot();
        }
    }
}
