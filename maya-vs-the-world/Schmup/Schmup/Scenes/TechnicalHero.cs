using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class TechnicalHero : Hero
    {
        private double elapsed;
        private double shootTime;

        public TechnicalHero (LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, int speed1, int speed2, Sprite skin = null)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, speed1, speed2, null)
        {

        }

        public override void Initialize()
        {
            elapsed = 0;
            shootTime = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Input.isActionDone(Input.Action.Shoot, true))
            {
                elapsed += gameTime.ElapsedGameTime.TotalSeconds;
                shootTime = 0.3;
            }
            else
            {
                elapsed = 0;
            }

            shootTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (shootTime > 0 && this.SpeedType)
            {
                WeakShoot();
            }
            else
            {
                shootTime = 0;
            }

            if (elapsed >= 0.15)
            {
                this.SpeedType = false;
            }
            else
            {
                this.SpeedType = true;
            }
            
        }
    }
}
