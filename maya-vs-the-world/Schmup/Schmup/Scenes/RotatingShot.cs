using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class RotatingShot : Shot
    {
        // CLASSE NON VALIDEE!

        private float rotation;
        private Vector2 direction;

        public RotatingShot(LuxGame game, World world, int invincibleTimeMillisec, bool isAGoodShot, float rotation, int hitbox, Sprite skin = null)
            : base(game, invincibleTimeMillisec, isAGoodShot, world, hitbox, 2, null)
        {
            this.rotation = rotation;
        }

        public void MemorizeDirection(Vector2 direction)
        {
            this.direction = direction;
        }

        public Vector2 GiveDirection()
        {
            return direction;
        }

        public override void Update(GameTime gameTime)
        {
            Speed = Vector2.Transform(Speed, Matrix.CreateRotationZ(rotation));
            base.Update(gameTime);
        }
    }
}
