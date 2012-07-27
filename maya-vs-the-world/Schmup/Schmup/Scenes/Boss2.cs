using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class Boss2 : Enemy
    {
        private int shootNb;
        private Texture2D bulletTexture;
        private List<RandomShotPattern> rspatterns = new List<RandomShotPattern>();
        private List<BigLockShotPattern> blspatterns = new List<BigLockShotPattern>();
        private Texture2D bulletTexture2;

        public Boss2(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, bool shootsHero, int waitTimeFrames, Sprite skin)
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
            for (int i = 0; i < 10; i++)
            {
                BigLockShotPattern bPatternTest2 = new BigLockShotPattern(this.LuxGame, i, 20, bulletTexture2, i+2, i, 2*i);
                Game.Components.Add(bPatternTest2);
                blspatterns.Add(bPatternTest2);
            }
            RandomShotPattern bPatternTestFinal = new RandomShotPattern(this.LuxGame, 120, 4 * vect, 3, bulletTexture, 15);
            Game.Components.Add(bPatternTestFinal);
            rspatterns.Add(bPatternTestFinal);
        }

        public void Shoot()
        {
            if (shootNb < 10)
            {
                blspatterns[(int)shootNb].Position = this.Position;
                blspatterns[(int)shootNb].Shoot(Common.HeroPosition - this.Position);
            }
            else
            {
                rspatterns[0].Position = this.Position;
                rspatterns[0].Shoot();
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            //this.Position += new Vector2(0,1);

            if (gameTime.TotalGameTime.Milliseconds.Equals(100) && shootNb < 11)
            {
                Shoot();
                shootNb++;
            }
        }
    }
}
