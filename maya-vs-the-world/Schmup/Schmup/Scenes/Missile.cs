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

        public Missile(LuxGame game, World world, int invincibleTimeMillisec)
            : base(game, world, invincibleTimeMillisec)
        {
        }
    }
}
