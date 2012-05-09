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
        private uint invincibleTimeMilisec;

        // méthodes pour update, draw et destroy??
        public uint InvincibleTimeMilisec
        {
            get
            {
                return invincibleTimeMilisec;
            }
            set
            {
                invincibleTimeMilisec = value;
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

        public Shot(LuxGame game, uint invincibleTimeMilisec)
            : base(game)
        {
            this.invincibleTimeMilisec = invincibleTimeMilisec;
        }
    }
}
