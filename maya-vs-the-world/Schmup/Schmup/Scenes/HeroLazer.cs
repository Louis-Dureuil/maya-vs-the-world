using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class HeroLazer : Shot
    {
        private int spriteNb;
        private List<Shot> lazers;
        private int currentLazerPart;
        private List<Texture2D> texture;
        private List<Vector2> vectors;

        public HeroLazer(LuxGame game, int invincibleTimeMillisec, int spriteNb, Sprite skin)
            : base(game, invincibleTimeMillisec, true, 10, skin)
        {
            this.spriteNb = spriteNb;
            texture = new List<Texture2D>(spriteNb);
            for (int i=0; i<spriteNb; i++)
            {
                Texture2D text = this.Content.Load<Texture2D>("lazermoche");
                this.texture.Add(text);
            }
        }

        public override void Initialize()
        {
            lazers = new List<Shot>(spriteNb);
            vectors = new List<Vector2>(spriteNb);
            currentLazerPart = 0;
            for (int i = 0; i < spriteNb; i++)
            {
                Vector2 vect = new Vector2(0, 0);
                vectors.Add(vect);
                Shot shot = new Shot(this.LuxGame, 0, true, 10, null);
                shot.Skin = new Sprite(shot, new List<Texture2D>() { texture[i] }, null);
                shot.Skin.SetAnimation(texture[i].Name);
                Game.Components.Add(shot);
                Game.Components.Add(shot.Skin);
                lazers.Add(shot);
            }
            base.Initialize();
        }

        public void Launch(int lazerIndex)
        {
            vectors[lazerIndex]=new Vector2(0,0);
            lazers[lazerIndex].Position = Position;
            lazers[lazerIndex].Shoot();
        }

        public void LazerDisable()
        {
            Vector2 hideVect = new Vector2(-42, -42);
            for (int i = 0; i < spriteNb; i++)
            {
                lazers[i].Position = hideVect;
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < currentLazerPart+1; i++)
            {
                // Gestion de la position et de la vitesse
                // Ces dernières doivent être relatives au lazer principal
                lazers[i].Position = Position + vectors[i];
                vectors[i] += new Vector2(0, -3);
            }
            
            if (Vector2.Distance(vectors[currentLazerPart],new Vector2(0,0)) > 32)
            {
                currentLazerPart = currentLazerPart% 19+1;
                if (currentLazerPart == 0)
                {
                    Launch(0);
                }
                Launch(currentLazerPart);
            }
            base.Update(gameTime);
        }
    }
}
