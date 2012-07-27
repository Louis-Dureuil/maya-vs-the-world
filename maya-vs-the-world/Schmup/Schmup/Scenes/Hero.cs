using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{

    class Hero : Character
    {
        private const float SQRT2 = 1.414f;

        private int mana;
        private bool cannotShoot;
        private int currentWeapon;
        private int speed;
        private int currentPower;
        private List<int> weaponList;
        private List<int> speedList;
        private List<int> powerList;
        private int invincibleTime;
        private int speed1;
        private int speed2;
        /// <summary>
        /// Détermine le type de vitesse
        /// </summary>
        private bool speedType;
        private int shootNb;
        private List<Shot> shots = new List<Shot>();
        private Texture2D bulletText;

        public Hero(LuxGame game, int life = 0, int takenDamageCollision = 0, int givenDamageCollision = 0, Sprite skin = null, int speed1 = 0, int speed2 = 0)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.speed1 = speed1;
            this.speed2 = speed2;
            this.bulletText = this.Content.Load<Texture2D>("bullet003-1");
        }

        public override void Initialize()
        {
            //base.Initialize();
            //Shot shot = new Shot(this.LuxGame, 0, null);
            //Sprite shotSprite = new Sprite(shot, new List<Texture2D>() { bulletText }, null);
            //shot.Speed = new Vector2(0, -10);
            //shot.Position = this.Position;
            //shot.Skin = shotSprite;
            //shotSprite.SetAnimation("bullet003-1");
            //Game.Components.Add(shot);
            //Game.Components.Add(shotSprite);
        }

        public void Shoot()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (Input.isActionDone(Input.Action.Cancel, true))
            {
                if (speedType)
                {
                    speedType = false;
                }
                else
                {
                    speedType = true;
                }
            }
            if (speedType)
            {
                speed = speed1;
            }
            else
            {
                speed = speed2;
            }
            // Gestion du déplacement
            if (Input.isActionDone(Input.Action.Up,true))
            {
                if (Input.isActionDone(Input.Action.Right, true))
                {
                    Position.X += speed / SQRT2;
                    Position.Y -= speed / SQRT2;
                }
                else if (Input.isActionDone(Input.Action.Left, true))
                {
                    Position.X -= speed / SQRT2;
                    Position.Y -= speed / SQRT2;
                }
                else
                {
                    Position.Y -= speed;
                }
            }
            else if (Input.isActionDone(Input.Action.Down, true))
            {
                if (Input.isActionDone(Input.Action.Right, true))
                {
                    Position.X += speed / SQRT2;
                    Position.Y += speed / SQRT2;
                }
                else if (Input.isActionDone(Input.Action.Left, true))
                {
                    Position.X -= speed / SQRT2;
                    Position.Y += speed / SQRT2;
                }
                else
                {
                    Position.Y += speed;
                }
            }
            else if (Input.isActionDone(Input.Action.Left, true))
            {
                Position.X -= speed;
            }
            else if (Input.isActionDone(Input.Action.Right, true))
            {
                Position.X += speed;
            }

            // Gestion des tirs

            if (Input.isActionDone(Input.Action.Confirm, true))
            {
                Shoot();
            }

            // Mise à jour de la position pour les ennemis
            Common.HeroPosition = this.Position;

            base.Update(gameTime);
        }
    }
}
