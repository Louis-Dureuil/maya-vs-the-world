using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class ShotPattern : LuxEngine.Scene
    {
        private uint shotNb;
        private Vector2 direction;
        private int angleBtwShotsDegrees;
        private Texture2D bulletText;
        private List<Shot> shots = new List<Shot>();

        public ShotPattern(LuxGame game, uint shotNb, Vector2 direction, int angleBtwShotsDegrees, Texture2D bulletText)
            : base(game)
        {
            this.shotNb = shotNb;
            this.direction = direction;
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
            this.bulletText = bulletText;
        }

        public void Direction(Vector2 direction)
        {
            this.direction = direction;
        }

        public override void Initialize()
        {
            base.Initialize();
            for (uint i = 0; i < shotNb; i++)
            {
                Shot shot = new Shot(this.LuxGame, 1, null);
                // TODO : Faire que le sprite soit crée dans le constructeur à partir du nom passé en paramètre
                Sprite shotSprite = new Sprite(shot, new List<Texture2D>() { bulletText }, null);
                shot.Skin = shotSprite;
                shot.Enabled = false;
                shotSprite.SetAnimation(bulletText.Name);
                Game.Components.Add(shot);
                Game.Components.Add(shotSprite);
                shots.Add(shot); 
            }
        }

        public void Shoot()
        {
            if (shotNb < 2)
            {
                return;
            }

            double bigAngleRadian = Math.PI / 180 * (shotNb - 1) * angleBtwShotsDegrees;
            Vector2 currentVector = direction;
            currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float)bigAngleRadian / 2));

            for (int i = 0; i < shotNb; i++)
            {
                shots[i].Speed = currentVector;
                shots[i].Position = this.Position;
                shots[i].Enabled = true;

                currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
            }
        }

        
    }
}
