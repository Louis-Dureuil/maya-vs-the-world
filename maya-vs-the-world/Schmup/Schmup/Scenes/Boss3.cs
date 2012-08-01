using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class Boss3 : Enemy
    {
        private int shootNb;
        private Texture2D bulletTexture;
        private List<RandomShotPattern> rspatterns = new List<RandomShotPattern>(600);
        private List<LockShotPattern> lspatterns = new List<LockShotPattern>(10);
        private Texture2D bulletTexture2;

        public Boss3(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, bool shootsHero, int waitTimeFrames, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {
            // A AMELIORER
            this.bulletTexture = this.Content.Load<Texture2D>("bullet001-1");
            this.bulletTexture2 = this.Content.Load<Texture2D>("bullet002-1");
        }

        public override void Initialize()
        {
            base.Initialize();
            shootNb = 0;
            Vector2 vect = new Vector2(0, 1);
            for (int i = 0; i < 600; i++)
            {
                RandomShotPattern bPatternTest2 = new RandomShotPattern(this.LuxGame, 2, vect, 40, bulletTexture2, 180);
                Game.Components.Add(bPatternTest2);
                rspatterns.Add(bPatternTest2);
            }
            RandomShotPattern bPatternTestFinal = new RandomShotPattern(this.LuxGame, 120, 4 * vect, 3, bulletTexture, 15);
            Game.Components.Add(bPatternTestFinal);
            rspatterns.Add(bPatternTestFinal);
            for (int i = 0; i < 20; i++)
            {
                LockShotPattern lspattern = new LockShotPattern(this.LuxGame, 3, 360, bulletTexture);
                Game.Components.Add(lspattern);
                lspatterns.Add(lspattern);
            }
        }

        public void Shoot()
        {
            if (shootNb < 600)
            {
                rspatterns[shootNb].Position = this.Position;
                rspatterns[shootNb].Shoot();
            }
            else if (shootNb == 600)
            {
                rspatterns[shootNb].Position = this.Position;
                rspatterns[shootNb].Shoot();
            }

            if (shootNb%30 == 0 && shootNb<600)
            {
                lspatterns[shootNb / 30].Position = this.Position;
                lspatterns[shootNb / 30].Shoot(7*Vector2.Normalize(Common.HeroPosition - this.Position));
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            Shoot();
            shootNb++;
        }
    }
}
