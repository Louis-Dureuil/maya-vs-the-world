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

        // hurtbox a améliorer
        private int hurtBox;
        // animation mort

        private Sprite skin;
        private World world;

        // Temps d'invincibilité
        // Peut exister pour certains ennemis coriaces

        private double invincibleTimeSec;

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

        public int Life
        {
            get
            {
                return life;
            }
        }

        public bool IsDead()
        {
            return (life <= 0);
        }

        public int GivenDamageCollision
        {
            get
            {
                return givenDamageCollision;
            }
        }

        public double InvincibleTimeSec
        {
            set
            {
                if (invincibleTimeSec == 0)
                {
                    invincibleTimeSec = value;
                }
            }
        }

        public int HurtBox
        {
            get
            {
                return hurtBox;
            }
        }


        // INTEGRER LA HURTBOX DANS LE CONSTRUCTEUR;
        public Character(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin)
            : base(game)
        {
            this.life = life;
            this.takenDamageCollision = takenDamageCollision;
            this.givenDamageCollision = givenDamageCollision;
            this.skin = skin;
            this.world = world;
        }

        public void Hurt(int lifeMinus)
        {
            life -= lifeMinus;
        }

        public void Collide(int collisionDamage)
        {
            life -= Math.Min(collisionDamage, takenDamageCollision);
        }

        public bool IsInvincible()
        {
            return (invincibleTimeSec > 0);
        }

        public virtual void Die()
        {
            // TODO : Ajouter l'animation de mort
            Enabled = false;
        }

        public override void Update(GameTime gameTime)
        {
            invincibleTimeSec -= gameTime.ElapsedGameTime.TotalSeconds;
            if (invincibleTimeSec < 0)
            {
                invincibleTimeSec = 0;
            }
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
