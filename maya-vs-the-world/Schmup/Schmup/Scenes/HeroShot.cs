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
        private bool isOutOfRange = true;

        public bool IsOutOfRange
        {
            get
            {
                return isOutOfRange;
            }
            set
            {
                isOutOfRange = value;
            }
        }


        public HeroShot(LuxGame game, int invincibleTimeMillisec, Sprite skin = null)
            : base(game, invincibleTimeMillisec, null)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            isOutOfRange = true;
            this.Enabled = false;
        }

        public void Shoot()
        {
            isOutOfRange = false;
            this.Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (this.Position.Y < 0)
            {
                isOutOfRange = true;
            }
        }
    }
}
