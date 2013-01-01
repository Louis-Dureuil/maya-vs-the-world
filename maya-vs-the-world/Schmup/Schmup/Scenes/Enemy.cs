using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class Enemy : Character
    {
        private int shotNb;
        private List<Shot> shots;
        private Texture2D bulletText;
        // L'ennemi va tirer uniquement des missiles du même type
        // avec la même hitbox
        private int shotHitbox;

        // Si rien n'est spécifié, il charge les tirs de base
        public Enemy(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision,
            int shotNb, Sprite skin)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.shotNb = shotNb;
            this.bulletText = this.Content.Load<Texture2D>("bullet001-1");
            this.shotHitbox = 8;
        }

        public Enemy(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, int shotNb,
            Sprite skin, Texture2D bulletText, int shotHitbox)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.shotNb = shotNb;
            this.bulletText = bulletText;
            this.shotHitbox = shotHitbox;
        }

        public override void Initialize()
        {
            base.Initialize();
            shots = new List<Shot>(shotNb);
            // Il va charger un certain nombre de tirs au départ
            // égal au maximum de balles qu'il y aura sur l'écran
            // (ce nombre est spécifié par l'utilisateur)
            for (int i = 0; i < shotNb; i++)
            {
                Shot shot = new Shot(this.LuxGame, 0, shotHitbox, null);
                shot.Skin = new Sprite(shot, new List<Texture2D>() { bulletText }, null);
                shot.Skin.SetAnimation(bulletText.Name);
                Game.Components.Add(shot);
                Game.Components.Add(shot.Skin);
                shots.Add(shot);
            }
            this.Enabled = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

    }
}
