using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class LockingEnemy : Enemy
    {
        private double elapsed;
        private List<BigLockShotPattern> blspats = new List<BigLockShotPattern>(3);
        private int shootNb;
        private Texture2D bulletTexture;

        public LockingEnemy(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin = null)
            : base(game, life, takenDamageCollision, givenDamageCollision, null)
        {
            this.bulletTexture = this.Content.Load<Texture2D>("bullet002-1");
        }

        public override void Initialize()
        {
            elapsed = 0;
            for (int i = 0; i < 3; i++)
            {
                BigLockShotPattern blspat = new BigLockShotPattern(this.LuxGame, 5, 1, bulletTexture, 7, 2 + i, 4 + (float)i/3);
                blspat.Position = this.Position;
                Game.Components.Add(blspat);
                blspats.Add(blspat);
            }
            shootNb = 0;
            base.Initialize();
        }



        public override void Update(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= 0.5 && shootNb < 3)
            {
                elapsed = 0;
                blspats[shootNb].Shoot(Common.HeroPosition - this.Position);
                shootNb++;
            }
            base.Update(gameTime);
        }
    }
}
