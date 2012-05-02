using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class ComplexEnemy : Enemy
    {
        // private List<uint> weaponList;

        public ComplexEnemy(LuxGame game, uint life, uint takenDamageCollision, uint givenDamageCollision, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {

        }
    }
}
