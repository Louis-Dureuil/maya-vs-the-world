using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class Character : LuxEngine.Scene
    {
        private int life;
        private int lifeStart;
        private int takenDamageCollision;
        private int givenDamageCollision;
        // hurtbox a definir
        // animation mort
        // skin
        private Sprite skin;

        public Character(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin)
            : base(game)
        {
            this.life = life;
            this.lifeStart = life;
            this.takenDamageCollision = takenDamageCollision;
            this.givenDamageCollision = givenDamageCollision;
            this.skin = skin;
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

        public void Hurt()
        {
            life -= 1;
        }

        public void LifeChange(int lifeMinus)
        {
            // a été fait par manque d'organisation
            // sert pour le boss, dont on ne tient pas compte des blessures à chaque fois
            life = lifeStart - lifeMinus;
        }

        public void Die()
        {
            // a corriger éventuellement
            // N'efface pas les balles
            Position = new Vector2(-400, -400);
            Enabled = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (life < 0)
            {
                Die();
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            skin.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
