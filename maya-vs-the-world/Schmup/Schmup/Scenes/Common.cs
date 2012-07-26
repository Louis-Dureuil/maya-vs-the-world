using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LuxEngine;

namespace Schmup
{
    class Common
    {
        private static Random rand = new Random();
        public static Random Rand
        {
        get { return rand; }
        }
        public static Vector2 HeroPosition;
    }
}
