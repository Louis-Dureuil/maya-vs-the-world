using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class FastBoss : Enemy
    {
        // elapsed sert de chronomètre
        private double elapsed;
        private Texture2D bigBulletText;
        // Liste de tirs à tête chercheuse
        private List<HomingMissile> homingShots = new List<HomingMissile>(20);
        // Liste de tirs rotatifs
        private List<RotatingShot> rotatingShots = new List<RotatingShot>(15);
        // shootOrder détermine quelle salve de tir envoyer
        private int shootOrder;
        private int secondShootOrder;
        // Variables spécifiques à une salve
        private double remainingTime;
        private float remainingTimeDeterminer;
        private double midElapsed;
        // rank augmente petit à petit, il détermine la puissance du boss
        private int rank;
        // On prend d'autres ennemis pour avoir des tirs différents
        private Enemy shooter;
        private Enemy bigShooter;
        private Enemy warning;
        private Texture2D enemyTexture;
        private Texture2D warningTexture;
        // Sert pour la salve spirale et la premiere
        private Vector2 currentDirectionVector;
        private Vector2 currentDirectionVector2;
        private float rotationAngle;
        private World world;

        public FastBoss(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin, Texture2D bulletText, World world, int shotHitbox)
            : base(game, life, takenDamageCollision, givenDamageCollision, 600, skin, bulletText, shotHitbox)
        {
            this.bigBulletText = this.Game.Content.Load<Texture2D>("bigbullet001-1");
            this.enemyTexture = this.Game.Content.Load<Texture2D>("commonEnemy");
            this.warningTexture = this.Game.Content.Load<Texture2D>("warning");
            this.world = world;
        }

        public override void Initialize()
        {
            elapsed = 0;
            shootOrder = 0;
            secondShootOrder = 0;
            rank = 0;
            shooter = new Enemy(this.LuxGame, 0, 0, 0, 600, null);
            shooter.Skin = new Sprite(shooter, new List<Texture2D>() { enemyTexture }, null);
            shooter.Skin.SetAnimation(enemyTexture.Name);
            Game.Components.Add(shooter);
            warning = new Enemy(this.LuxGame, 0, 0, 0, 0, null);
            warning.Skin = new Sprite(warning, new List<Texture2D>() { warningTexture }, null);
            warning.Skin.SetAnimation(warningTexture.Name);
            warning.Position = new Vector2(-40, -40);
            Game.Components.Add(warning);
            bigShooter = new Enemy(this.LuxGame, 0, 0, 0, 200, null, bigBulletText, 20);
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
            for (int i = 0; i < 15; i++)
            {
                RotatingShot rotatingShot = new RotatingShot(this.LuxGame, world, 0, false, (float)0.02, 20, null);
                rotatingShot.Skin = new Sprite(rotatingShot, new List<Texture2D>() { bigBulletText }, null);
                rotatingShot.Skin.SetAnimation(bigBulletText.Name);
                Game.Components.Add(rotatingShot);
                Game.Components.Add(rotatingShot.Skin);
                rotatingShots.Add(rotatingShot);
            }
            Game.Components.Add(bigShooter);
            rotationAngle = (float)0.02;
            currentDirectionVector = new Vector2(0, (float)2.5);
            remainingTimeDeterminer = 0;
            remainingTime = 0.3 * Math.Cos(remainingTimeDeterminer);
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

        public void RotateShoot()
        {
            double bigAngleRadian = Math.PI / 180 * (14) * 24;
            Vector2 currentDirectionVector = new Vector2(0, 4);
            int deviationDegrees = Common.Rand.Next(-20, 20);
            float deviationRadian = (float)Math.PI / 180 * deviationDegrees;
            currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ(deviationRadian));
            currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ((float)bigAngleRadian / 2));
            for (int i = 0; i < 15; i++)
            {
                rotatingShots[i].Position = Position;
                rotatingShots[i].Speed = currentDirectionVector;
                rotatingShots[i].Accel = (float)0.025 * currentDirectionVector;
                rotatingShots[i].MemorizeDirection(Vector2.Transform((float)0.8 * rotatingShots[i].Speed, Matrix.CreateRotationZ((float)0.3)));
                rotatingShots[i].Shoot();
                currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ((float)-bigAngleRadian / 14));
            }
        }

        public override void Update(GameTime gameTime)
        {
            // elapsed est incrémenté à chaque frame
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;
            midElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            // On veille à ce que les ennemis attachés au boss le restent
            shooter.Position = this.Position;
            bigShooter.Position = this.Position;
            for (int i = 0; i < 89; i++)
            {
                if (elapsed >= 2 + 0.15 * i && shootOrder == i)
                {
                    shootOrder++;
                    for (int j = 0; j < 15; j++)
                    {
                        SuperShoot(rotatingShots[j].Position, rotatingShots[j].GiveDirection(), new Vector2(0, 0));
                    }
                    if (shootOrder % 10 == 0)
                    {
                        RotateShoot();
                    }
                }
            }
            // Deuxième salve de tir
            if (secondShootOrder < 250)
            {
                currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ(rotationAngle));
            }
            for (int i = 0; i < 250; i++)
            {
                if (midElapsed >= remainingTime && secondShootOrder == i)
                {
                    shooter.RandomPatternShoot(Position, currentDirectionVector, new Vector2(0, 0), 45, 8, 0);
                    midElapsed = 0;
                    remainingTimeDeterminer++;
                    remainingTime = 0.15 * Math.Cos(0.2 * remainingTimeDeterminer);
                    secondShootOrder++;
                }
            }

            if (secondShootOrder == 250 && shootOrder == 89)
            {
                currentDirectionVector = 5 * Vector2.Normalize(currentDirectionVector);
                rotationAngle = Common.Rand.Next(0, 100);
            }

            // salve n°2

            for (int i = 0; i < 161; i++)
            {
                if (elapsed >= 15 + i * 0.05 && shootOrder == 89 + i && secondShootOrder == 250)
                {
                    currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ(rotationAngle));
                    rotationAngle += (float)0.004;
                    bigShooter.RandomPatternShoot(this.Position, currentDirectionVector, new Vector2(0, 0), 120, 3, 0);
                    if (shootOrder % 30 == 10)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            shooter.RandomPatternShoot(Position + new Vector2(-30 + 60 * j, -20),
                                (3 + (float)0.05 * shootOrder) * Vector2.Normalize(world.GetHero().Position - Position - new Vector2(-30 + 60 * j, -20)),
                                new Vector2(0, 0), 4 - (float)0.01 * shootOrder, 7, 0);
                        }
                    }
                    shootOrder++;
                }
            }

            //salve n°3

            if (shootOrder == 250 && secondShootOrder == 250)
            {
                rotationAngle = (float)0.005;
                currentDirectionVector = new Vector2(0, (float)3.5);
                remainingTimeDeterminer = 0;
                remainingTime = 0.5 * Math.Cos(remainingTimeDeterminer);
            }
            for (int i = 0; i < 100; i++)
            {
                if (elapsed >= 22.5 + 0.15 * i && shootOrder == 250 + i)
                {
                    shootOrder++;
                    if (shootOrder % 9 == 0)
                    {
                        for (int j = 0; j < 15; j++)
                        {
                            SuperShoot(rotatingShots[j].Position,
                                2* Vector2.Normalize(world.GetHero().Position - rotatingShots[j].Position),
                                (float)0.03 * Vector2.Normalize(world.GetHero().Position - rotatingShots[j].Position));
                        }
                    }
                    if (shootOrder % 9 == 5)
                    {
                        RotateShoot();
                    }
                }
            }
            // Deuxième salve de tir
            if (secondShootOrder > 249 && secondShootOrder < 400 && shootOrder > 249 && shootOrder <350)
            {
                currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ(rotationAngle));
            }
            for (int i = 0; i < 150; i++)
            {
                if (midElapsed >= remainingTime && secondShootOrder == i+250 && shootOrder > 249)
                {
                    shooter.RandomPatternShoot(Position, currentDirectionVector, new Vector2(0, 0), 45, 8, 0);
                    midElapsed = 0;
                    remainingTimeDeterminer++;
                    remainingTime = 0.3 * Math.Cos(0.2 * remainingTimeDeterminer);
                    secondShootOrder++;
                }
            }

            // salve n°4

            if (elapsed >= 38)
            {
                rotationAngle = (float)0.1;
            }
            if (shootOrder == 350)
            {
                currentDirectionVector = new Vector2(0, 3);
            }
            for (int i = 0; i < 50; i++)
            {
                if (elapsed >= 38 + 0.2 * i && shootOrder == 350 + i)
                {
                    shootOrder++;
                    currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ(rotationAngle));
                    for (int j = 0; j<4; j++)
                    {
                        Vector2 vect = (float)(4 + j) / 3 * Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ((float)j * (float)0.1));
                        shooter.SuperShoot(Position, vect, new Vector2(0, 0));
                        shooter.SuperShoot(Position, new Vector2(-vect.X,vect.Y), new Vector2(0, 0));
                        shooter.SuperShoot(Position, new Vector2(-vect.X, -vect.Y), new Vector2(0, 0));
                        shooter.SuperShoot(Position, new Vector2(vect.X, -vect.Y), new Vector2(0, 0));
                    }
                }
            }

            for (int i = 0; i < 100; i++)
            {
                if (midElapsed >= 0.1 && secondShootOrder == 400 + i)
                {
                    midElapsed = 0;
                    secondShootOrder++;
                    if (secondShootOrder % 8 == 0)
                    {
                        currentDirectionVector2 = 5 * Vector2.Normalize(world.GetHero().Position - Position - new Vector2(20, 20));
                    }
                    else if (secondShootOrder % 8 == 4)
                    {
                        currentDirectionVector2 = 5 * Vector2.Normalize(world.GetHero().Position - Position - new Vector2(-20, 20));
                    }
                    if (secondShootOrder % 8 < 4)
                    {
                        SuperShoot(Position + new Vector2(20, 20), currentDirectionVector2, new Vector2(0, 0));
                    }
                    else 
                    {
                        SuperShoot(Position + new Vector2(-20, 20), currentDirectionVector2, new Vector2(0, 0));
                    }
                }
            }

            base.Update(gameTime);
        }
    }
}
