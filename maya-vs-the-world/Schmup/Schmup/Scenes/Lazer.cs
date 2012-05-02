using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class Lazer : Shot
    {
        //hitbox

        public Lazer(LuxGame game, uint invincibleTime)
            : base(game, invincibleTime)
        {
        }
    }
}
