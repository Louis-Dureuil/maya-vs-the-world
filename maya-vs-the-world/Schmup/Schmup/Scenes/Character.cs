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
        private uint life;
        private uint takenDamageCollision;
        private uint givenDamageCollision;
        // hurtbox a definir
        // animation mort
        // skin
        private Sprite skin;

        public Character(LuxGame game, uint life, uint takenDamageCollision, uint givenDamageCollision, Sprite skin)
            : base(game)
        {
            this.life = life;
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            skin.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
