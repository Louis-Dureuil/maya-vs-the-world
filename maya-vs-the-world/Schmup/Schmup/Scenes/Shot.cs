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
        //private uint hitboxHeight;
        //private uint hitboxWidth;
        private int invincibleTimeMillisec;
        private Vector2 speed;
        private Vector2 accel;
        private Sprite skin;

        // méthodes pour update, draw et destroy??
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
            accel = new Vector2(0, 0);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Position += speed;
            speed += accel;
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
        }
    }
}
