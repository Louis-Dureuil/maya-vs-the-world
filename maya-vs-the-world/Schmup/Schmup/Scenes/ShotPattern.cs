using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class ShotPattern : LuxEngine.Scene
    {
        private uint shotNb;
        private Vector2 direction;
        private uint angleBtwShotsDegrees;

        public ShotPattern(LuxGame game, uint shotNb, Vector2 direction, uint angleBtwShotsDegrees)
            : base(game)
        {
            this.shotNb = shotNb;
            this.direction = direction;
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            double bigAngleRadian = Math.PI/180*(shotNb - 1) * angleBtwShotsDegrees;
            Vector2 currentVector = direction;
            currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float) bigAngleRadian/2));
            List<string> skinName = new List<string>(1);
            skinName.Add("bullet001-1");
            for (uint i = 0; i < shotNb; i++)
            {
                Shot shot = new Shot(this.LuxGame, 1, null);
                // TODO : Faire que le sprite soit crée dans le constructeur à partir du nom passé en paramètre
                Sprite shotSprite = new Sprite(shot, skinName);
                shot.Speed = currentVector;
                shot.Position = this.Position;
                shot.Skin = shotSprite;
                shotSprite.SetAnimation("bullet001-1");
                Game.Components.Add(shot);
                Game.Components.Add(shotSprite);
                System.Console.WriteLine(currentVector);
                currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float) - bigAngleRadian /(shotNb-1)));
            }
        }


        
    }
}
