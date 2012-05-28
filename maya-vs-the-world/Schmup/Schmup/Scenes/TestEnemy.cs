using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LuxEngine;

namespace Schmup
{
    class TestEnemy : Enemy
    {
        private bool shootsHero;
        private uint waitTimeFrames;
        private uint shootNb;
        private float shootSpeed;

        public TestEnemy (LuxGame game, uint life, uint takenDamageCollision, uint givenDamageCollision, bool shootsHero, uint waitTimeFrames, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.shootsHero = shootsHero;
            this.waitTimeFrames = waitTimeFrames;
        }

        public override void Initialize()
        {
            base.Initialize();
            List<string> skinName = new List<string>(1);
            skinName.Add("bullet001-1");
            Vector2 vector = new Vector2(0,1);
            if (shootsHero)
            {
                Vector2 vect = new Vector2(1, 1);
            }
            Shot shot = new Shot(this.LuxGame, 1, null);
            Sprite shotSprite = new Sprite(shot, skinName);
            shot.Speed = vector;
            shot.Skin = shotSprite;
            shot.Position = this.Position;

            Game.Components.Add(shot);
            Game.Components.Add(shotSprite);
            shotSprite.SetAnimation("bullet001-1");
            shootNb=1;
            shootSpeed = (float) 0.05;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (shootNb < 30)
            {
                if (waitTimeFrames == 0)
                {
                    shootNb++;
                    shootSpeed = (float) 1.1 * shootSpeed + (float) 0.1;
                    Vector2 vector = new Vector2(0,(float) shootSpeed);
                    if (shootsHero)
                    {
                        Vector2 vect = new Vector2(1, 1);
                    }
                    ShotPattern bPatternTest = new ShotPattern(this.LuxGame, 7, vector, 12);
                    bPatternTest.Position = this.Position;
                    Game.Components.Add(bPatternTest);
                    waitTimeFrames += 3;
                }
                waitTimeFrames--;
            }
        }
    }
}
