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
        private List<RandomShotPattern> spatterns = new List<RandomShotPattern>();

        public Boss(LuxGame game, uint life, uint takenDamageCollision, uint givenDamageCollision, bool shootsHero, uint waitTimeFrames, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.bulletTexture = this.Content.Load<Texture2D>("bullet001-1");
        }

        public override void Initialize()
        {
            base.Initialize();
            shootNb = 0;
            for (int i = 0; i < 20; i++)
            {
                //Vector2 vect = new Vector2(0, (float)i/4);
                Vector2 vect = new Vector2(0, 1);
                RandomShotPattern bPatternTest = new RandomShotPattern(this.LuxGame, 20, vect, 5, bulletTexture, i);
                Game.Components.Add(bPatternTest);
                spatterns.Add(bPatternTest);
            }
        }

        public void Shoot()
        {
            spatterns[(int)shootNb].Position = this.Position;
            spatterns[(int)shootNb].Shoot();
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
