using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class ShotPull : Scene
    {
        // Sert pour savoir la quantité de tirs utilisés
        private int maxActiveShots;
        private int shotNb;
        private List<Shot> allShots;
        private List<Shot> activeShots;
        private List<Shot> nonActiveShots;
        private World world;
        private Texture2D bulletText;
        private int shotHitBox;
        private int damage;
        bool isAGoodShot;

        // Si rien n'est spécifié, les tirs auront une texture prédéfinie
        public ShotPull(LuxGame game, World world)
            : base(game)
        {
            this.world = world;
            maxActiveShots = 0;
            shotNb = 20;
            shotHitBox = 8;
            bulletText = this.Content.Load<Texture2D>("bullet001-2");
            allShots = new List<Shot>(shotNb);
            activeShots = new List<Shot>(shotNb);
            nonActiveShots = new List<Shot>(shotNb);
        }

        public override void Initialize()
        {
            base.Initialize();
            for (int i = 0; i < shotNb; i++)
            {
                Shot shot = new Shot(LuxGame, world, 1, null);
                shot.Skin = new Sprite(shot, new List<Texture2D>() { bulletText }, null);
                shot.Skin.SetAnimation(bulletText.Name);
                Game.Components.Add(shot);
                Game.Components.Add(shot.Skin);
                allShots.Add(shot);
                nonActiveShots.Add(shot);
            }
            System.Console.WriteLine("fin du modele");
        }

        private void activeShotsAdd(Shot shot)
        {
            activeShots.Add(shot);
            if (isAGoodShot)
            {
                world.GoodShots.Add(shot);
            }
            else
            {
                world.BadShots.Add(shot);
            }
        }

        private void activeShotsRemove(Shot shot)
        {
            nonActiveShots.Add(shot);
            activeShots.Remove(shot);
            if (isAGoodShot)
            {
                world.GoodShots.Remove(shot);
            }
            else
            {
                world.BadShots.Remove(shot);
            }
        }

        private void shoot(Shot shot, float positionX, float positionY,
            float speedX, float speedY, float accelX, float accelY,
            bool copyPosition, bool copySpeed, bool copyAccel)
        {
            if (copyPosition)
            {
                shot.Position.X = positionX;
                shot.Position.Y = positionY;
            }
            else
            {
                shot.Position = new Vector2(positionX, positionY);
            }
            if (copySpeed)
            {
                shot.Speed.X = speedX;
                shot.Speed.Y = speedY;
            }
            else
            {
                shot.Speed = new Vector2(speedX,speedY);
            }
            if (copyAccel)
            {
                shot.Accel.X = accelX;
                shot.Accel.Y = accelY;
            }
            else
            {
                shot.Accel = new Vector2(accelX, accelY);
            }
            shot.Shoot();
        }

        public void Shoot(float positionX, float positionY,
            float speedX, float speedY, float accelX, float accelY,
            bool copyPosition, bool copySpeed, bool copyAccel)
        {
            if (shotNb == maxActiveShots)
            {
                shotNb++;
                maxActiveShots++;
                Shot shot = new Shot(LuxGame, world, 1, null);
                shot.Skin = new Sprite(shot, new List<Texture2D>() { bulletText }, null);
                shot.Skin.SetAnimation(bulletText.Name);
                Game.Components.Add(shot);
                Game.Components.Add(shot.Skin);
                allShots.Add(shot);
                activeShotsAdd(shot);
                shoot(shot,positionX,positionY,speedX,speedY,accelX,accelY,copyPosition,copySpeed,copyAccel);
            }
            else
            {
                shoot(nonActiveShots[0],positionX,positionY,speedX,speedY,accelX,accelY,copyPosition,copySpeed,copyAccel);
                activeShotsAdd(nonActiveShots[0]);
                nonActiveShots.Remove(nonActiveShots[0]);
                maxActiveShots++;
            }
        }

        public void Print()
        {
            System.Console.WriteLine("Nombre total de tirs  : " + maxActiveShots + "/" + shotNb);
        }

        public void Check()
        {
            foreach (Shot shot in activeShots.ToList<Shot>())
            {
                if (shot.IsOutOfRange)
                {
                    activeShotsRemove(shot);
                    maxActiveShots--;
                }
            }
            System.Console.WriteLine("Avant triage");
        }
    }
}
