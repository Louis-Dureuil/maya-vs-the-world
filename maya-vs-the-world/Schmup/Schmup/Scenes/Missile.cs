using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class Missile:Shot
    {
        private uint degat;
        //skin

        public Missile(LuxGame game, uint invincibleTime)
            : base(game, invincibleTime)
        {
        }
    }
}
