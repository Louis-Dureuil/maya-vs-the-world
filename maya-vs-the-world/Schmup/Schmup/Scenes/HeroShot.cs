using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class HeroShot : Shot
    {
        // CLASSE NON VALIDEE!

        public HeroShot(LuxGame game, World world, int invincibleTimeMillisec, bool goesThrough, int damage, Sprite skin = null)
            : base(game, invincibleTimeMillisec, true, goesThrough, world, 1, damage, null)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
