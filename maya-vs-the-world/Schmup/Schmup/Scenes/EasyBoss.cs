using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class EasyBoss : Enemy
    {
        // CLASSE NON VALIDEE!

        // elapsed sert de chronomètre
        private double elapsed;
        private Texture2D bigBulletText;
        // Liste de tirs à tête chercheuse
        private List<HomingMissile> homingShots = new List<HomingMissile>(20);
        // shootOrder détermine quelle salve de tir envoyer
        private int shootOrder;
        // rank augmente petit à petit, il détermine la puissance du boss
        private int rank;
        // On prend d'autres ennemis pour avoir des tirs différents
        private Enemy shooter;
        private Enemy bigShooter;
        private Enemy warning;
        private Texture2D enemyTexture;
        private Texture2D warningTexture;
        // Sert pour la salve finale (la spirale)
        private Vector2 currentDirectionVector;
        private Vector2 currentAccelVector;
        private float rotationAngle;

        public EasyBoss(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin, Texture2D bulletText, int shotHitbox)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, 600, skin, bulletText, shotHitbox)
        {
            this.bigBulletText = this.Game.Content.Load<Texture2D>("bigbullet001-1");
            this.enemyTexture = this.Game.Content.Load<Texture2D>("commonEnemy");
            this.warningTexture = this.Game.Content.Load<Texture2D>("warning");
        }

        public override void Initialize()
        {
            elapsed = 0;
            shootOrder = 0;
            rank = 0;
            shooter = new Enemy(LuxGame, World, 0, 0, 0, 600, null);
            shooter.Skin = new Sprite(shooter, new List<Texture2D>() { enemyTexture }, null);
            shooter.Skin.SetAnimation(enemyTexture.Name);
            Game.Components.Add(shooter);
            warning = new Enemy(LuxGame, World, 0, 0, 0, 0, null);
            warning.Skin = new Sprite(warning, new List<Texture2D>() { warningTexture }, null);
            warning.Skin.SetAnimation(warningTexture.Name);
            warning.Position = new Vector2(-40, -40);
            Game.Components.Add(warning);
            bigShooter = new Enemy(LuxGame, World, 0, 0, 0, 20, null, bigBulletText, 20);
            bigShooter.Skin = new Sprite(bigShooter, new List<Texture2D>() { enemyTexture }, null);
            bigShooter.Skin.SetAnimation(enemyTexture.Name);
            for (int i = 0; i < 20; i++)
            {
                HomingMissile homingShoot = new HomingMissile(this.LuxGame, 0, false, (float)0.05, 20, null);
                homingShoot.Skin = new Sprite(homingShoot, new List<Texture2D>() { bigBulletText }, null);
                homingShoot.Skin.SetAnimation(bigBulletText.Name);
                Game.Components.Add(homingShoot);
                Game.Components.Add(homingShoot.Skin);
                homingShots.Add(homingShoot);
            }
            Game.Components.Add(bigShooter);
            rotationAngle = Common.Rand.Next(0, 300);
            currentDirectionVector = new Vector2(0, (float)1.5);
            currentAccelVector = new Vector2(1, 0);
            base.Initialize();
        }


        public void HomingShoot(Vector2 position, Vector2 direction, Vector2 accel)
        {
            int cnt = 0;
            bool fini = false;

            while (cnt < 20 && !fini)
            {
                if (homingShots[cnt].IsOutOfRange == true)
                {
                    homingShots[cnt].Speed = direction;
                    homingShots[cnt].Position = position;
                    homingShots[cnt].Accel = accel;
                    homingShots[cnt].Shoot();
                    fini = true;
                }
                else
                {
                    cnt++;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // elapsed est incrémenté à chaque frame
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;
            // On veille à ce que les ennemis attachés au boss le restent
            shooter.Position = this.Position;
            bigShooter.Position = this.Position;
            base.Update(gameTime);

            // On utilise une fonction de elapsed, shootOrder, et rank
            // Comme le timing entre plusieurs évènements est différent, il faut elapsed ET
            // shootOrder pour savoir où on en est sans tirer la mauvaise salve
            if (elapsed >= 1.5 && shootOrder == 0)
            {
                LongPatternShoot(this.Position, new Vector2(0, 1), new Vector2(0, 0), 4 + rank, new Vector2(0, (float)0.3), new Vector2(0, (float)0.01), 30 - 3 * rank, 4 + rank, 5, 2);
                shootOrder++;
            }
            if (elapsed >= 2.4 && shootOrder == 1)
            {
                LongPatternShoot(this.Position, new Vector2(0, 1), new Vector2(0, 0), 5 + rank, new Vector2(0, (float)0.3 - (float)0.04 * rank), new Vector2(0, (float)0.01), 30 - 3 * rank, 5 + rank, 5, 3 - (float)0.2 * rank);
                shootOrder++;
            }
            if (elapsed >= 3.5 && shootOrder == 2)
            {
                LongPatternShoot(this.Position, new Vector2(0, 1), new Vector2(0, 0), 7 + rank, new Vector2(0, (float)0.4 - (float)0.02 * rank), new Vector2(0, (float)0.01), 30 - 3 * rank, 6 + rank, 5, 4 - (float)0.3 * rank);
                shootOrder++;
            }
            if (elapsed >= 4.5 && shootOrder == 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    shooter.RandomPatternShoot(this.Position + new Vector2(40 - 80 * i, 0), (5 + rank) * Vector2.Normalize(World.Hero.Position - this.Position - new Vector2(40 - 80 * i, 0)), -(float)0.015 * Vector2.Normalize(World.Hero.Position - this.Position - new Vector2(40 - 80 * i, 0)), 25, 5, 0);
                }
                for (int i = 0; i < 2; i++)
                {
                    shooter.RandomPatternShoot(this.Position + new Vector2(40 - 80 * i, 0), (5 + rank) * Vector2.Normalize(World.Hero.Position - this.Position - new Vector2(40 - 80 * i, 0)), -(float)0.005 * Vector2.Normalize(World.Hero.Position - this.Position - new Vector2(40 - 80 * i, 0)), 1, 6*rank, 0);
                }
                shootOrder++;
            }
            if (elapsed >= 5.7 && shootOrder == 4)
            {
                warning.Position = new Vector2(400, 300);
                shootOrder++;
            }
            if (elapsed >= 6.6 && shootOrder == 5)
            {
                warning.Position = new Vector2(-40, -40);
                for (int i = 0; i < 7 + 3 * rank; i++)
                {
                    Vector2 vect = new Vector2((float)0.1 * Common.Rand.Next(-9 - 5 * rank, 10 + 5 * rank), 8 + (float)0.1 * Common.Rand.Next(-9 - 5 * rank, 10 + 5 * rank));
                    bigShooter.Shoot(vect);
                }
                shootOrder++;
            }
            for (int i = 0; i < 30; i++)
            {
                if (elapsed >= 8 + i * 0.2 && shootOrder == 6 + i)
                {
                    RandomPatternShoot(this.Position, new Vector2(0, (float)1.5), new Vector2(0, 0), 360 / (2 + 4 * rank), 2 + 4 * rank, 180);
                    shooter.RandomPatternShoot(this.Position, new Vector2(0, (float)1.5 + rank), new Vector2(0, 0), 45 - 8 * rank, 2 + 4 * rank, 6);
                    shootOrder++;
                    if (rank > 1 && shootOrder % 10 == 0 && shootOrder < 40 && shootOrder > 0)
                    {
                        shooter.BigPatternShoot(World.Hero.Position - this.Position, 10, 1, 2, 5, (float)5.2);
                    }
                }
            }
            for (int i = 0; i < 20; i++)
            {
                if (elapsed >= 15 + i * 0.4 && shootOrder == 36 + i)
                {
                    HomingShoot(this.Position + new Vector2(40 * (2 * (i % 2) - 1), 20), (1 + (float)0.1 * rank) * new Vector2(2 * (i % 2) - 1, -2), new Vector2(0, 0));
                    shootOrder++;
                }
            }
            for (int i = 0; i < 80; i++)
            {
                if (elapsed >= 27 + i * 0.2 && shootOrder == 56 + i)
                {
                    currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ(rotationAngle));
                    currentAccelVector = Vector2.Transform(currentAccelVector, Matrix.CreateRotationZ(2 * rotationAngle));
                    rotationAngle += (float)0.01;
                    shooter.RandomPatternShoot(this.Position, (1 + (float)0.2 * rank) * currentDirectionVector, (float)0.005 * rank * currentAccelVector, 360 / (6 + 4 * rank), 6 + 4 * rank, 0);
                    shootOrder++;
                }
            }
            if (elapsed >= 44 && shootOrder == 136)
            {
                // Le boss recommence son pattern de 44 secondes,
                // mais un pattern plus fort, car rank := rank + 1
                elapsed = 0;
                shootOrder = 0;
                if (rank < 5)
                {
                    rank++;
                }
            }

            //Déplacement du boss
            // A FAIRE

        }
    }
}
