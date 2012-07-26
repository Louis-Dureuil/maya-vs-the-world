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
        public Vector2 HeroPosition;

        public Boss(LuxGame game, uint life, uint takenDamageCollision, uint givenDamageCollision, bool shootsHero, uint waitTimeFrames, Sprite skin, Vector2 HeroPosition)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.bulletTexture = this.Content.Load<Texture2D>("bullet001-1");
            this.HeroPosition = HeroPosition;
        }

        public override void Initialize()
        {
            base.Initialize();
            shootNb = 0;
            for (int i = 0; i < 20; i++)
            {
                //Vector2 vect = new Vector2(0, (float)i/4);
                Vector2 vect = new Vector2(0, 1);
                RandomShotPattern bPatternTest = new RandomShotPattern(this.LuxGame, 72, 3*vect, 5, bulletTexture, 15);
                Game.Components.Add(bPatternTest);
                rspatterns.Add(bPatternTest);
                //LockShotPattern bPatternTest2 = new LockShotPattern(this.LuxGame, 20, vect, 10, bulletTexture, 2*Vector2.Normalize(HeroPosition - this.Position));
                //Game.Components.Add(bPatternTest2);
                //lspatterns.Add(bPatternTest2);
            }
        }

        public void Shoot()
        {
            rspatterns[(int)shootNb].Position = this.Position;
            rspatterns[(int)shootNb].Shoot();
            //lspatterns[(int)shootNb].Position = this.Position;
            //lspatterns[(int)shootNb].Shoot(2 * Vector2.Normalize(HeroPosition - this.Position));
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            if (gameTime.TotalGameTime.Milliseconds == 100 && shootNb < 20)
            {
                Shoot();
                shootNb++;
            }
        }
    }
}
