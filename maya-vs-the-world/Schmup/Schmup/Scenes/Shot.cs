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
        private Vector2 speed;
        private Vector2 accel;
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

        public Vector2 Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        public Vector2 Accel
        {
            get
            {
                return accel;
            }
            set
            {
                accel = value;
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

                Position += speed;
                speed += accel;
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

        public Shot(LuxGame game, World world, int invincibleTimeMillisec, Sprite skin = null)
            : base(game)
        {
            this.world = world;
            this.invincibleTimeSec = invincibleTimeMillisec;
            this.skin = skin;
            this.isABadShot = true;
            this.hitbox = 3;
        }

        public Shot(LuxGame game, int invincibleTimeMillisec, bool isAGoodShot, bool goesThrough, World world, int hitbox, Sprite skin = null)
            : base(game)
        {
            this.invincibleTimeSec = invincibleTimeMillisec;
            this.skin = skin;
            this.isABadShot = !isAGoodShot;
            this.goesThrough = goesThrough;
            this.hitbox = hitbox;
            this.world = world;
        }

        public Shot(LuxGame game, int invincibleTimeMillisec, int hitbox, World world, Sprite skin = null)
            : base(game)
        {
            this.invincibleTimeSec = invincibleTimeMillisec;
            this.skin = skin;
            this.isABadShot = true;
            this.hitbox = hitbox;
            this.world = world;
        }
    }
}
