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

        // Les deux int qui suivent désignent les dommages qu'un type peut causer, et ceux qu'il peut encaisser lors d'une collision.
        // Exemple : Un gros vaisseau en métal va pas avoir mal lors d'une collision,
        // Mais c'est pas pour autant qu'il va vous démolir! Il y a plein d'autres exemples.
        private int takenDamageCollision;
        private int givenDamageCollision;

        // hurtbox a definir
        // animation mort
        private Sprite skin;

        private World world;

        public World World
        {
            get
            {
                return world;
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


        public Character(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin)
            : base(game)
        {
            this.life = life;
            this.takenDamageCollision = takenDamageCollision;
            this.givenDamageCollision = givenDamageCollision;
            this.skin = skin;
            this.world = world;
        }

        public void Hurt()
        {
            life -= 1;
        }

        public void Die()
        {
            // TODO : Ajouter l'animation de mort
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
