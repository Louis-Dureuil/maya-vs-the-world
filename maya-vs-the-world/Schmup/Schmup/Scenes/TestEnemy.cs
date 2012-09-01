using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LuxEngine;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class TestEnemy : Enemy
    {
        private bool shootsHero;
        private int waitTimeFrames;
        private int shootNb;
        private float shootSpeed;
        private Vector2 speed = new Vector2((float)2, (float)1);
        private Vector2 accel = new Vector2(0, (float)-0.01);
        private List<ShotPattern> spatterns = new List<ShotPattern>();

        // Ces textures seront utilisées tout le temps pendant le combat, et doivent donc être préchargées.
        private Texture2D bulletTexture;

        public TestEnemy(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, bool shootsHero, int waitTimeFrames, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, 20, skin)
        {
            this.shootsHero = shootsHero;
            this.waitTimeFrames = waitTimeFrames;
            this.bulletTexture = this.Content.Load<Texture2D>("bullet001-1");
        }


        public TestEnemy(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, bool shootsHero, int waitTimeFrames, Sprite skin, Texture2D bulletTexture)
            : base(game, life, takenDamageCollision, givenDamageCollision, 20, skin)
        {
            this.shootsHero = shootsHero;
            this.waitTimeFrames = waitTimeFrames;
            this.bulletTexture = bulletTexture;
        }

        public override void Initialize()
        {
            base.Initialize();
            List<string> skinName = new List<string>(1);
            skinName.Add("bullet001-1");
            if (shootsHero)
            {
                Vector2 vect = new Vector2(1, 1);
            }
            shootNb = 1;
            shootSpeed = (float)0.5;
            for (int i = 0; i < 20; i++)
            {
                    shootSpeed = (float)1.1 * shootSpeed + (float)0.15;
                    Vector2 vector = new Vector2(0, (float)shootSpeed);
                    if (shootsHero)
                    {
                        Vector2 vect = new Vector2(1, 1);
                    }
                    ShotPattern bPatternTest = new ShotPattern(this.LuxGame, 4, vector, 40, bulletTexture);
                    Game.Components.Add(bPatternTest);
                    spatterns.Add(bPatternTest);
            }
        }

        public void Shoot()
        {
            spatterns[(int)shootNb].Position = this.Position;
            spatterns[(int)shootNb].Shoot(); 
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.Position += speed;
            this.speed += accel;
            if (shootNb < 20)
            {
                if (waitTimeFrames == 0)
                {
                    //bPatternTest.Position = this.Position;
                    Shoot();
                    shootNb++;

                    waitTimeFrames += 4;
                }
            }
            waitTimeFrames--;
        }
    }
}
