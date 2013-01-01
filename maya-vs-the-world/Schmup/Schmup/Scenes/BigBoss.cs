using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class BigBoss : Enemy
    {
        private int shootNb;
        private Texture2D bulletTexture;
        private Texture2D bulletTexture2;
        private Texture2D bulletTexture3;
        private Texture2D enemyTexture;
        private double elapsed;
        private double midElapsed;
        //List<RotatingEnemy> commonEnemies = new List<RotatingEnemy>(6);
        Enemy shooter;

        public BigBoss(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, bool shootsHero, int waitTimeFrames, Sprite skin, Texture2D bulletText)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, 650, skin, bulletText, 8)
        {
            // A AMELIORER
            this.bulletTexture = this.Content.Load<Texture2D>("bullet001-1");
            this.bulletTexture2 = this.Content.Load<Texture2D>("bullet002-1");
            this.bulletTexture3 = this.Content.Load<Texture2D>("bullet005-1");
            this.enemyTexture = this.Content.Load<Texture2D>("commonEnemy");
        }

        public override void Initialize()
        {
            base.Initialize();
            shootNb = 0;
            shooter = new Enemy(LuxGame, World, 10, 10, 10, 300, null, bulletTexture, 8);
            shooter.Skin = new Sprite(shooter, new List<Texture2D>() { enemyTexture }, null);
            shooter.Skin.SetAnimation(enemyTexture.Name);
            shooter.Position = new Vector2(400, 100);
            Game.Components.Add(shooter);
            Vector2 vect = Vector2.Normalize(new Vector2(Common.Rand.Next(-2, 2), 10));
            //for (int i = 0; i < 2; i++)
            //{
            //    commonEnemies.Add(new RotatingEnemy(this.LuxGame, 1, 1, 1, null, 50, -10 + 20 * i, vect, 0.2, 4, 90, 150));
            //    commonEnemies[i].Skin = new Sprite(commonEnemies[i], new List<Texture2D>() { enemyTexture }, null);
            //    commonEnemies[i].Skin.SetAnimation(enemyTexture.Name);
            //    commonEnemies[i].Position = new Vector2(300 + i * 200, 100);
            //    // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.
            //    Game.Components.Add(commonEnemies[i]);
            //}
            vect = new Vector2(0, 1);
            //for (int i = 2; i < 4; i++)
            //{
            //    commonEnemies.Add(new RotatingEnemy(this.LuxGame, 1, 1, 1, null, 90, 11 - 22 * (i - 2), (float)1.5 * vect, 0.2, 4, 90, 100));
            //    commonEnemies[i].Skin = new Sprite(commonEnemies[i], new List<Texture2D>() { enemyTexture }, null);
            //    commonEnemies[i].Skin.SetAnimation(enemyTexture.Name);
            //    commonEnemies[i].Position = new Vector2(300 + (i - 2) * 200, 100);
            //    // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.
            //    Game.Components.Add(commonEnemies[i]);
            //}
            //for (int i = 4; i < 6; i++)
            //{
            //    int horiz = Common.Rand.Next(1, 4);
            //    commonEnemies.Add(new RotatingEnemy(this.LuxGame, 1, 1, 1, null, 260, 2 * (9 - 18 * (i - 4)) / 5, (float)2.5 * Vector2.Normalize(new Vector2(horiz * (1 - 2 * (i - 4)), 10)), 0.05, 2, 180, bulletTexture3, 120, 5));
            //    commonEnemies[i].Skin = new Sprite(commonEnemies[i], new List<Texture2D>() { enemyTexture }, null);
            //    commonEnemies[i].Skin.SetAnimation(enemyTexture.Name);
            //    commonEnemies[i].Position = new Vector2(400, 80);
            //    // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.
            //    Game.Components.Add(commonEnemies[i]);
            //}
        }

        public void RandomShoot()
        {
            if (shootNb > 11 && shootNb < 611)
            {
                shootNb++;
                Vector2 vect = new Vector2(Common.Rand.Next(-30, 30), Common.Rand.Next(-20, 20));
                RandomPatternShoot(this.Position + vect, (1 + (float)0.1 * Common.Rand.Next(0, 3)) * Vector2.Normalize(vect), new Vector2(0, 0), 180, 2, 180);
            }
        }

        public void Shooting()
        {
            if (shootNb % 30 == 0 && shootNb < 601)
            {
                shooter.Shoot(7 * Vector2.Normalize(World.Hero.Position - shooter.Position));
            }
        }

        public void DoubleShoot()
        {
            if (midElapsed >= 1 && shootNb > 611 && shootNb < 622)
            {
                midElapsed = 0;
                shootNb++;
                shooter.RandomPatternShoot(this.Position, new Vector2(3, 0), new Vector2(0, 0), 5, 72, 5);
                BigPatternShoot(World.Hero.Position - this.Position, 5, Common.Rand.Next(2, 10), 2, (float)4.8, (float)5.2);
            }
            if (midElapsed >= 1 && shootNb == 622)
            {
                shootNb++;
                shooter.RandomPatternShoot(this.Position, new Vector2(4, 0), new Vector2(0, 0), 3, 120, 3);
            }
        }

        public void LockingShoot()
        {
            if (midElapsed >= 0.6 && shootNb > 622 && shootNb < 626)
            {
                midElapsed = 0;
                shootNb++;
                BigPatternShoot(World.Hero.Position - this.Position, 5, 4, (shootNb - 622)+1, (float)3.5, (float)3.5 + (shootNb - 622));
                BigPatternShoot(World.Hero.Position - this.Position, 4, 4, (shootNb - 622), 4, 4 + (shootNb - 623));
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;
            midElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < 11; i++)
            {
                if (elapsed >= i + 0.1 && shootNb == i)
                {
                    BigPatternShoot(World.Hero.Position - this.Position, shootNb + 1, 20, shootNb - 1, 1, (float)shootNb / (float)2);
                    shootNb++;
                }
            }
            if (elapsed >= 11.1 && shootNb == 11)
            {
                shooter.RandomPatternShoot(this.Position, new Vector2(0, 4), new Vector2(0, 0), 3, 120, 3);
                shootNb++;
            }

            //commonEnemies[0].Create();
            //commonEnemies[1].Create();
            if (elapsed >= 15.5)
            {
                RandomShoot();
                Shooting();
            }
            if (elapsed >= 24.7 && shootNb == 611)
            {
                shooter.RandomPatternShoot(this.Position, new Vector2(0, 4), new Vector2(0, 0), 3, 120, 3);
                shootNb++;
            }
            if (elapsed >= 32)
            {
                DoubleShoot();
            }
            if (elapsed >= 45)
            {
                for (int i = 2; i < 6; i++)
                {
                    //commonEnemies[i].Create();
                }
            }
            if (elapsed >= 60)
            {
                LockingShoot();
            }
            if (elapsed >= 62 && shootNb == 626)
            {
                shootNb++;
                shooter.RandomPatternShoot(this.Position, new Vector2(1, 0), new Vector2(0, (float)0.07), 5, 72, 5);
                shooter.RandomPatternShoot(this.Position, new Vector2(0, 4), new Vector2(0, 0), 4, 90, 4);
            }
            if (elapsed >= 65)
            {
                elapsed = 0;
                shootNb = 0;
                for (int i = 0; i < 6; i++)
                {
                    //commonEnemies[i].Reset();
                }
            }
        }
    }
}
