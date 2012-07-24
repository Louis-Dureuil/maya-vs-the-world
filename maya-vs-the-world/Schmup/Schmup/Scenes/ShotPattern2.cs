using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class ShotPattern2 : LuxEngine.Scene
    {
        private uint shotNb;
        private Vector2 direction;
        private uint angleBtwShotsDegrees;

        public ShotPattern2(LuxGame game, uint shotNb, Vector2 direction, uint angleBtwShotsDegrees)
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
            Shot[] shotAr = new Shot[shotNb];
            Sprite[] shotSpriteAr = new Sprite[shotNb];
            for (uint i = 0; i < shotNb; i++)
            {
                shotAr[i] = new Shot(this.LuxGame, 1, null);
                // TODO : Faire que le sprite soit crée dans le constructeur à partir du nom passé en paramètre
                shotSpriteAr[i] = new Sprite(shotAr[i], skinName);
                shotAr[i].Speed = currentVector;
                shotAr[i].Position = this.Position;
                shotAr[i].Skin = shotSpriteAr[i];
                shotSpriteAr[i].SetAnimation("bullet001-1");
                Game.Components.Add(shotAr[i]);
                Game.Components.Add(shotSpriteAr[i]);
                System.Console.WriteLine(currentVector);
                currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float) - bigAngleRadian /(shotNb-1)));
            }
        }


        
    }
}
