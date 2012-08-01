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
        private List<RandomShotPattern> rspatterns = new List<RandomShotPattern>();
        private List<BigLockShotPattern> blspatterns = new List<BigLockShotPattern>();
        private Texture2D bulletTexture2;
        private Texture2D bulletTexture3;
        private Texture2D enemyTexture;
        private double elapsed;
        List<RotatingEnemy> commonEnemies = new List<RotatingEnemy>(2);
        Boss bossAlt;
        Boss3 bossFinal;
        LockingEnemy enemy;

        public BigBoss(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, bool shootsHero, int waitTimeFrames, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
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
            Vector2 vect = new Vector2(0, 1);
            for (int i = 0; i < 10; i++)
            {
                BigLockShotPattern bPatternTest2 = new BigLockShotPattern(this.LuxGame, i+2, 20, bulletTexture2, i, 1, (i+2)/2);
                Game.Components.Add(bPatternTest2);
                blspatterns.Add(bPatternTest2);
            }
            RandomShotPattern bPatternTestFinal = new RandomShotPattern(this.LuxGame, 120, 4 * vect, 3, bulletTexture, 15);
            Game.Components.Add(bPatternTestFinal);
            rspatterns.Add(bPatternTestFinal);
            for (int i = 0; i < 2; i++)
            {
                commonEnemies.Add(new RotatingEnemy(this.LuxGame, 1, 1, 1, null, 60, -10 + 20 * i, new ShotPattern(this.LuxGame, 4, new Vector2(0, 1), 30, bulletTexture2), 0.2, 4, 90));
                commonEnemies[i].Skin = new Sprite(commonEnemies[i], new List<Texture2D>() { enemyTexture }, null);
                commonEnemies[i].Skin.SetAnimation(enemyTexture.Name);
                commonEnemies[i].Position = new Vector2(300 + i * 200, 100);
                // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.
                Game.Components.Add(commonEnemies[i]);
            }
            bossAlt = new Boss(this.LuxGame, 10, 10, 10, false, 1, null);
            bossAlt.Skin = new Sprite(bossAlt, new List<string>() { "boss" });
            bossAlt.Skin.SetAnimation("boss");
            bossAlt.Position = new Vector2(400, 50);
            bossAlt.Enabled = false;
            bossFinal = new Boss3(this.LuxGame, 10, 10, 10, false, 1, null);
            bossFinal.Skin = new Sprite(bossFinal, new List<string>() { "boss" });
            bossFinal.Skin.SetAnimation("boss");
            bossFinal.Position = new Vector2(400, 50);
            bossFinal.Enabled = false;
            Game.Components.Add(bossAlt);
            Game.Components.Add(bossFinal);
            for (int i = 2; i < 4; i++)
            {
                commonEnemies.Add(new RotatingEnemy(this.LuxGame, 1, 1, 1, null, 90, 11 - 22 * (i-2), new ShotPattern(this.LuxGame, 4, new Vector2(0, (float)1.21), 30, bulletTexture2), 0.2, 4, 90));
                commonEnemies[i].Skin = new Sprite(commonEnemies[i], new List<Texture2D>() { enemyTexture }, null);
                commonEnemies[i].Skin.SetAnimation(enemyTexture.Name);
                commonEnemies[i].Position = new Vector2(280 + (i-2) * 240, 100);
                commonEnemies[i].Enabled = false;
                // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.
                Game.Components.Add(commonEnemies[i]);
            }
            for (int i= 4 ; i < 6; i++)
            {
                int horiz = Common.Rand.Next(1, 4);
                commonEnemies.Add(new RotatingEnemy(this.LuxGame, 1, 1, 1, null, 260, 2*(9 - 18 * (i-4))/5, new ShotPattern(this.LuxGame, 4, (float)2.5*Vector2.Normalize(new Vector2(horiz *(1-2*(i-4)), 10)), 30, bulletTexture3), 0.05, 2, 180, bulletTexture3));
                commonEnemies[i].Skin = new Sprite(commonEnemies[i], new List<Texture2D>() { enemyTexture }, null);
                commonEnemies[i].Skin.SetAnimation(enemyTexture.Name);
                commonEnemies[i].Position = new Vector2(400, 80);
                commonEnemies[i].Enabled = false;
                // Il faut appliquer "SetAnimation" au sprite pour qu'il affiche quelque chose.
                Game.Components.Add(commonEnemies[i]);
            }
            enemy = new LockingEnemy(this.LuxGame, 1, 1, 1, null);
            enemy.Skin = new Sprite(enemy, new List<Texture2D>() { enemyTexture }, null);
            enemy.Skin.SetAnimation(enemyTexture.Name);
            enemy.Position = new Vector2(400, 80);
            enemy.Enabled = false;
            Game.Components.Add(enemy);
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
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= 14)
            {
                bossAlt.Enabled = true;
            }
            if (elapsed >= 26)
            {
                bossFinal.Enabled = true;
            }
            if (elapsed >= 40)
            {
                for (int i = 2; i < 6; i++)
                {
                    commonEnemies[i].Enabled = true;
                }
            }
            if (elapsed >= 48)
            {
                enemy.Enabled = true;
            }
        }
    }
}
