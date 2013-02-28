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
        // CLASSE NON VALIDEE!

        // elapsed sert de chronomètre
        private double elapsed;
        private Texture2D bigBulletText;
        private Texture2D bulletText;
        private int shotHitBox;
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
        private Texture2D enemyTexture;
        // Sert pour la salve spirale et la premiere
        private Vector2 currentDirectionVector;
        private Vector2 currentDirectionVector2;
        private float rotationAngle;

        public FastBoss(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin, Texture2D bulletText, World world, int shotHitBox)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.bulletText = bulletText;
            this.shotHitBox = shotHitBox;
            this.bigBulletText = this.Game.Content.Load<Texture2D>("bigbullet001-1");
            this.enemyTexture = this.Game.Content.Load<Texture2D>("commonEnemy");
        }

        public override void Initialize()
        {
            elapsed = 0;
            shootOrder = 0;
            secondShootOrder = 0;
            rank = 0;
            ShotPull redShots = new ShotPull(LuxGame, World, false, false, 0.3, 100, 100, shotHitBox, 15, bulletText, null);
            Game.Components.Add(redShots);
            ShotsOfType.Add(redShots);
            ShotPull blueShots = new ShotPull(LuxGame, World);
            shooter = new Enemy(LuxGame, World, 0, 0, 0, new List<ShotPull>() {blueShots}, null);
            shooter.Skin = new Sprite(shooter, new List<Texture2D>() { enemyTexture }, null);
            shooter.Skin.SetAnimation(enemyTexture.Name);
            Game.Components.Add(blueShots);
            Game.Components.Add(shooter);
            ShotPull bigShots = new ShotPull(LuxGame, World, false, false, 1.5, 150, 200, 20, 50, bigBulletText, null);
            bigShooter = new Enemy(LuxGame, World, 0, 0, 0, new List<ShotPull>() {bigShots}, null);
            bigShooter.Skin = new Sprite(bigShooter, new List<Texture2D>() { enemyTexture }, null);
            bigShooter.Skin.SetAnimation(enemyTexture.Name);
            Game.Components.Add(bigShots);
            for (int i = 0; i < 20; i++)
            {
                HomingMissile homingShoot = new HomingMissile(this.LuxGame, 0, false, (float)0.05, 20, 7, World, null);
                homingShoot.Skin = new Sprite(homingShoot, new List<Texture2D>() { bigBulletText }, null);
                homingShoot.Skin.SetAnimation(bigBulletText.Name);
                Game.Components.Add(homingShoot);
                Game.Components.Add(homingShoot.Skin);
                homingShots.Add(homingShoot);
                World.BadShots.Add(homingShoot);
            }
            for (int i = 0; i < 15; i++)
            {
                RotatingShot rotatingShot = new RotatingShot(this.LuxGame, World, 0, false, (float)0.02, 10, null);
                rotatingShot.Skin = new Sprite(rotatingShot, new List<Texture2D>() { bigBulletText }, null);
                rotatingShot.Skin.SetAnimation(bigBulletText.Name);
                Game.Components.Add(rotatingShot);
                Game.Components.Add(rotatingShot.Skin);
                rotatingShots.Add(rotatingShot);
                World.BadShots.Add(rotatingShot);
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

        public override void Die()
        {
            shooter.Die();
            Position = new Vector2(-200, -200);
            ShotsOfType[0].Clear();
            shooter.ShotsOfType[0].Clear();
            bigShooter.ShotsOfType[0].Clear();
            base.Die();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            // Précautions de départ
            // TODO : a améliorer
            if (elapsed == 0)
            {
                shooter.Activate();
                bigShooter.Activate();
            }

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
                        SuperShoot(0, rotatingShots[j].Position, rotatingShots[j].GiveDirection(), new Vector2(0, 0));
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
                    shooter.RandomPatternShoot(0, Position, currentDirectionVector, new Vector2(0, 0), 45, 8, 0);
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
                    bigShooter.RandomPatternShoot(0, this.Position, currentDirectionVector, new Vector2(0, 0), 120, 3, 0);
                    if (shootOrder % 30 == 10)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            shooter.RandomPatternShoot(0, Position + new Vector2(-30 + 60 * j, -20),
                                (3 + (float)0.05 * shootOrder) * Vector2.Normalize(this.World.Hero.Position - Position - new Vector2(-30 + 60 * j, -20)),
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
                            SuperShoot(0, rotatingShots[j].Position,
                                2* Vector2.Normalize(this.World.Hero.Position - rotatingShots[j].Position),
                                (float)0.03 * Vector2.Normalize(this.World.Hero.Position - rotatingShots[j].Position));
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
                    shooter.RandomPatternShoot(0, Position, currentDirectionVector, new Vector2(0, 0), 45, 8, 0);
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
                        shooter.SuperShoot(0, Position, vect, new Vector2(0, 0));
                        shooter.SuperShoot(0, Position, new Vector2(-vect.X,vect.Y), new Vector2(0, 0));
                        shooter.SuperShoot(0, Position, new Vector2(-vect.X, -vect.Y), new Vector2(0, 0));
                        shooter.SuperShoot(0, Position, new Vector2(vect.X, -vect.Y), new Vector2(0, 0));
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
                        currentDirectionVector2 = 5 * Vector2.Normalize(this.World.Hero.Position - Position - new Vector2(20, 20));
                    }
                    else if (secondShootOrder % 8 == 4)
                    {
                        currentDirectionVector2 = 5 * Vector2.Normalize(this.World.Hero.Position - Position - new Vector2(-20, 20));
                    }
                    if (secondShootOrder % 8 < 4)
                    {
                        SuperShoot(0, Position + new Vector2(20, 20), currentDirectionVector2, new Vector2(0, 0));
                    }
                    else 
                    {
                        SuperShoot(0, Position + new Vector2(-20, 20), currentDirectionVector2, new Vector2(0, 0));
                    }
                }
            }
        }
    }
}
