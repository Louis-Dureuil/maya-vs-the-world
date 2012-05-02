using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class Shot : Scene
    {
        //private uint hitboxHeight;
        //private uint hitboxWidth;
        private uint invincibleTime;

        // méthodes pour update, draw et destroy??
        public uint InvincibleTime
        {
            get
            {
                return invincibleTime;
            }
            set
            {
                invincibleTime = value;
            }
        }

        public override void Initialize()
        {
            
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public Shot(LuxGame game, uint invincibleTime)
            : base(game)
        {
            this.invincibleTime = invincibleTime;
        }
    }
}
