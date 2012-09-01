using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class Boss : Enemy
    {
        private int shootNb;
        private Texture2D bulletTexture;
        private List<RandomShotPattern> rspatterns = new List<RandomShotPattern>();
        private List<LockShotPattern> lspatterns = new List<LockShotPattern>();
        private Texture2D bulletTexture2;

        public Boss(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, bool shootsHero, int waitTimeFrames, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, 20, skin)
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
            for (int i = 0; i < 9; i++)
            {
                RandomShotPattern bPatternTest = new RandomShotPattern(this.LuxGame, 72, 3*vect, 5, bulletTexture, 15);
                Game.Components.Add(bPatternTest);
                rspatterns.Add(bPatternTest);
                LockShotPattern bPatternTest2 = new LockShotPattern(this.LuxGame, 5, Common.Rand.Next(2, 10), bulletTexture2);
                Game.Components.Add(bPatternTest2);
                lspatterns.Add(bPatternTest2);
            }
            RandomShotPattern bPatternTestFinal = new RandomShotPattern(this.LuxGame, 120, 4 * vect, 3, bulletTexture, 15);
            Game.Components.Add(bPatternTestFinal);
            rspatterns.Add(bPatternTestFinal);
            LockShotPattern bPatternFinal2 = new LockShotPattern(this.LuxGame, 0, 40, bulletTexture2);
            Game.Components.Add(bPatternFinal2);
            lspatterns.Add(bPatternFinal2);
        }

        public void Shoot()
        {
            rspatterns[(int)shootNb].Position = this.Position;
            rspatterns[(int)shootNb].Shoot();
            lspatterns[(int)shootNb].Position = this.Position;
            lspatterns[(int)shootNb].Shoot(5 * Vector2.Normalize(Common.HeroPosition - this.Position));
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            //this.Position += new Vector2(0,1);

            if (gameTime.TotalGameTime.Milliseconds.Equals(100) && shootNb < 10)
            {
                Shoot();
                shootNb++;
            }
        }
    }
}
