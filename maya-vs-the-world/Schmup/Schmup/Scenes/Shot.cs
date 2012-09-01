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
        private int hitbox;
        private int invincibleTimeMillisec;
        private Vector2 speed;
        private Vector2 accel;
        private Sprite skin;
        private bool isOutOfRange;
        private bool goesThrough;
        private bool isABadShot;

        public bool GoesThrough
        {
            get
            {
                return goesThrough;
            }
            set
            {
                goesThrough = value;
            }
        }

        public bool IsOutOfRange
        {
            get
            {
                return isOutOfRange;
            }
            set
            {
                isOutOfRange = value;
            }
        }


        public int InvincibleTimeMillisec
        {
            get
            {
                return invincibleTimeMillisec;
            }
            set
            {
                invincibleTimeMillisec = value;
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

        public override void Initialize()
        {
            Position = new Vector2(-40, -40);
            this.Enabled = false;
            isOutOfRange = true;
            goesThrough = false;
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

                if (this.Position.Y < -30)
                {
                    isOutOfRange = true;
                }
                if (this.Position.Y > 500)
                {
                    isOutOfRange = true;
                }
                if (this.Position.X < -30)
                {
                    isOutOfRange = true;
                }
                if (this.Position.X > 830)
                {
                    isOutOfRange = true;
                }

                //Gestion d'une collision avec le héros

                if (Vector2.Distance(Position, Common.HeroPosition) < hitbox && isABadShot)
                {
                    Common.HeroHit++;
                    System.Console.Write("Tu t'es fait frapper. Il te reste ");
                    System.Console.Write(10 - Common.HeroHit);
                    System.Console.WriteLine(" tentatives.");

                    Position = new Vector2(-40, -40);
                    speed = new Vector2(0, 0);
                    accel = new Vector2(0, 0);
                }

                //Gestion d'une collision avec le pouvoir

                if (Vector2.Distance(Position, Common.PowerPosition) < 22 && isABadShot)
                {
                    Common.PowerHit++;

                    Position = new Vector2(-40, -40);
                    speed = new Vector2(0, 0);
                    accel = new Vector2(0, 0);
                }

                //Gestion d'une collision avec le boss

                if (Vector2.Distance(Position, Common.BossPosition) < 60 && !isABadShot)
                {
                    Common.BossHit++;

                    if (!goesThrough)
                    {
                        Position = new Vector2(-40, -40);
                        speed = new Vector2(0, 0);
                        accel = new Vector2(0, 0);
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public Shot(LuxGame game, int invincibleTimeMillisec, Sprite skin = null)
            : base(game)
        {
            this.invincibleTimeMillisec = invincibleTimeMillisec;
            this.skin = skin;
            this.isABadShot = true;
            this.hitbox = 3;
        }

        public Shot(LuxGame game, int invincibleTimeMillisec, bool isAGoodShot, int hitbox, Sprite skin = null)
            : base(game)
        {
            this.invincibleTimeMillisec = invincibleTimeMillisec;
            this.skin = skin;
            this.isABadShot = !isAGoodShot;
            this.hitbox = hitbox;
        }

        public Shot(LuxGame game, int invincibleTimeMillisec, int hitbox, Sprite skin = null)
            : base(game)
        {
            this.invincibleTimeMillisec = invincibleTimeMillisec;
            this.skin = skin;
            this.isABadShot = true;
            this.hitbox = hitbox;
        }
    }
}
