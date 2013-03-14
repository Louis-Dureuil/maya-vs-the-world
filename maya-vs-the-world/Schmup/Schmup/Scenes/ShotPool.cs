using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class ShotPool : Scene
    {
        // Sert pour savoir la quantité de tirs utilisés
        private int maxActiveShots;
        // Nombre de tirs initialement utilisés
        private int shotNb;
        // Nombre de tirs maximum acceptés sur l'écran
        private int maxShotNb;
        private List<Shot> allShots;
        private List<Shot> activeShots;
        private List<Shot> nonActiveShots;
        private World world;
        private Texture2D bulletText;
        private int shotHitBox;
        private int damage;
        private bool isAGoodShot;
        private double invincibleTimeSec;

        public List<Shot> ActiveShots
        {
            get
            {
                return activeShots;
            }
        }

        // Si rien n'est spécifié, les tirs auront une texture prédéfinie
        public ShotPool(LuxGame game, World world)
            : base(game)
        {
            this.world = world;
            isAGoodShot = false;
            maxActiveShots = 0;
            invincibleTimeSec = 0.1;
            // TODO : BAISSER CE CHIFFRE
            // Créer d'autres constructeurs plus utiles
            shotNb = 400;
            maxShotNb = 400;
            shotHitBox = 8;
            damage = 10;
            bulletText = this.Content.Load<Texture2D>("bullet001-1");
            allShots = new List<Shot>(shotNb);
            activeShots = new List<Shot>(shotNb);
            nonActiveShots = new List<Shot>(shotNb);
        }

        public ShotPool(LuxGame game, World world, bool isAGoodShot, 
            double invincibleTimeSec, int shotNb, int maxShotNb, int shotHitBox,
            int damage, Texture2D bulletText, Sprite skin = null)
            : base(game)
        {
            this.world = world;
            this.isAGoodShot = isAGoodShot;
            maxActiveShots = 0;
            this.invincibleTimeSec = invincibleTimeSec;
            this.shotNb = shotNb;
            this.maxShotNb = maxShotNb;
            this.shotHitBox = shotHitBox;
            this.damage = damage;
            this.bulletText = bulletText;
            allShots = new List<Shot>(shotNb);
            activeShots = new List<Shot>(shotNb);
            nonActiveShots = new List<Shot>(shotNb);
        }

        // TODO : Implémenter un pseudo constructeur par recopie
        public ShotPool(LuxGame game, World world, Shot shot, int shotNb, int maxShotNb, Sprite skin = null)
            : base(game)
        {
            this.world = world;
            // A COMPLETER
        }

        public override void Initialize()
        {
            base.Initialize();
            for (int i = 0; i < shotNb; i++)
            {
                Shot shot = new Shot(LuxGame, invincibleTimeSec, isAGoodShot, world, shotHitBox, damage, null);
                shot.Skin = new Sprite(shot, new List<Texture2D>() { bulletText }, null);
                shot.Skin.SetAnimation(bulletText.Name);
                Game.Components.Add(shot);
                Game.Components.Add(shot.Skin);
                allShots.Add(shot);
                nonActiveShots.Add(shot);
            }
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
                shot.Speed = new Vector2(speedX, speedY);
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

        /// <summary>
        /// Tire une balle, la fait rentrer en compte dans le monde pour les collisions
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="speedX"></param>
        /// <param name="speedY"></param>
        /// <param name="accelX"></param>
        /// <param name="accelY"></param>
        /// <param name="copyPosition"></param>
        /// <param name="copySpeed"></param>
        /// <param name="copyAccel"></param>
        public void Shoot(float positionX, float positionY,
            float speedX, float speedY, float accelX, float accelY,
            bool copyPosition, bool copySpeed, bool copyAccel)
        {
            if (shotNb == maxActiveShots)
            {
                if (shotNb == maxShotNb)
                {
                    return;
                }
                shotNb++;
                maxActiveShots++;
                Shot shot = new Shot(LuxGame, world, 1, null);
                shot.Skin = new Sprite(shot, new List<Texture2D>() { bulletText }, null);
                shot.Skin.SetAnimation(bulletText.Name);
                Game.Components.Add(shot);
                Game.Components.Add(shot.Skin);
                allShots.Add(shot);
                activeShotsAdd(shot);
                shoot(shot, positionX, positionY, speedX, speedY, accelX, accelY, copyPosition, copySpeed, copyAccel);
            }
            else
            {
                shoot(nonActiveShots[0], positionX, positionY, speedX, speedY, accelX, accelY, copyPosition, copySpeed, copyAccel);
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
        }

        public void Clear()
        {
            foreach (Shot shot in activeShots.ToList<Shot>())
            {
                shoot(shot, -40, -40, 0, 0, 0, 0, false, false, false);
                activeShotsRemove(shot);
                maxActiveShots--;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.isActionDone(Input.Action.Confirm, false))
            {
                System.Console.WriteLine("Etat des balles : " + maxActiveShots + "/" + shotNb + "/" + maxShotNb);
            }
            base.Update(gameTime);
        }
    }
}
