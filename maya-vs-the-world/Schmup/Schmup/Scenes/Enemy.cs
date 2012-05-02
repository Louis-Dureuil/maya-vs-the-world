using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class Enemy : Character
    {
        // private uint weapon;
        // weapon a changer

        public Enemy(LuxGame game, uint life, uint takenDamageCollision, uint givenDamageCollision, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {

        }
    }
}
