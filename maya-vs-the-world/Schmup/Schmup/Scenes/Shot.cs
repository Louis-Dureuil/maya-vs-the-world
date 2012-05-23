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
        private uint invincibleTimeMilisec;
        private Vector2 speed;
        private Sprite skin;

        // méthodes pour update, draw et destroy??
        public uint InvincibleTimeMilisec
        {
            get
            {
                return invincibleTimeMilisec;
            }
            set
            {
                invincibleTimeMilisec = value;
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

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Position = Position + speed;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public Shot(LuxGame game, uint invincibleTimeMilisec, Sprite skin = null)
            : base(game)
        {
            this.invincibleTimeMilisec = invincibleTimeMilisec;
            this.skin = skin;
        }
    }
}
