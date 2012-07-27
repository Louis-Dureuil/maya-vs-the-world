using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class RotatingEnemy : Enemy
    {

        public RotatingEnemy(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            for (int i = 0; i < 20; i++)
            {

            }
        }
    }
}
