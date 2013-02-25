using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class Shot : Scene
    {
        private float hitbox;
        private double invincibleTimeSec;
        private int damage;
        public Vector2 Speed;
        public Vector2 Accel;
        private Sprite skin;
        private bool isOutOfRange;
        private bool goesThrough;
        private bool isABadShot;
        private World world;

        public bool GoesThrough
        {
            get
            {
                return goesThrough;
            }
        }

        public bool IsOutOfRange
        {
            get
            {
                return isOutOfRange;
            }
        }


        public double InvincibleTimeSec
        {
            get
            {
                return invincibleTimeSec;
            }
        }

        public Sprite Skin
        {
            get
            {
                return skin;
            }
            set
            {
                skin = value;
            }
        }

        public float Hitbox
        {
            get
            {
                return hitbox;
            }
        }

        public int Damage
        {
            get
            {
                return damage;
            }
        }

        public override void Initialize()
        {
            Position = new Vector2(-40, -40);
            this.Enabled = false;
            isOutOfRange = true;
            base.Initialize();
        }

        public void Shoot()
        {
            isOutOfRange = false;
            this.Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (isOutOfRange == true)
            {
                Position = new Vector2(-40, -40);
            }
            else
            {

                Position += Speed;
                Speed += Accel;
                //if (speed.Length() < 1)
                //{
                //    accel = new Vector2(0,0);
                //    speed = Vector2.Normalize(speed);
                //}

                // Gestion de l'atteinte aux bordures
                // TODO : Mettre des variables globales

                if (this.Position.Y < -hitbox)
                {
                    isOutOfRange = true;
                }
                if (this.Position.Y > 480 + hitbox)
                {
                    isOutOfRange = true;
                }
                if (this.Position.X < -hitbox)
                {
                    isOutOfRange = true;
                }
                if (this.Position.X > 800 + hitbox)
                {
                    isOutOfRange = true;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public Shot(LuxGame game, World world, double invincibleTimeSec, Sprite skin = null)
            : base(game)
        {
            this.world = world;
            this.invincibleTimeSec = invincibleTimeSec;
            this.skin = skin;
            this.isABadShot = true;
            this.hitbox = 3;
            this.damage = 1;
        }

        public Shot(LuxGame game, double invincibleTimeSec, bool isAGoodShot, bool goesThrough, World world, int hitbox, int damage, Sprite skin = null)
            : base(game)
        {
            this.invincibleTimeSec = invincibleTimeSec;
            this.skin = skin;
            this.isABadShot = !isAGoodShot;
            this.goesThrough = goesThrough;
            this.hitbox = hitbox;
            this.world = world;
            this.damage = damage;
        }

        public Shot(LuxGame game, double invincibleTimeSec, int hitbox, int damage, World world, Sprite skin = null)
            : base(game)
        {
            this.invincibleTimeSec = invincibleTimeSec;
            this.skin = skin;
            this.isABadShot = true;
            this.hitbox = hitbox;
            this.world = world;
            this.damage = damage;
        }
    }
}
