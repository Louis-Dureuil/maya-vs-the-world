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
            direction.Normalize();
            this.direction = direction;
            System.Console.WriteLine(this.direction);
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
        }

        public override void Initialize()
        {
            base.Initialize();
            // double beta = (Math.Atan((double) fin.X / (double) fin.Y) - Math.Atan((double) debut.X / (double) debut.Y))/(nbTirs-1);

            double bigAngleRadian = Math.PI/180*(shotNb - 1) * angleBtwShotsDegrees;
            Vector2 currentVector = new Vector2 ((float) (direction.X + Math.Cos(bigAngleRadian/2)), (float) (direction.Y - Math.Sin((float) bigAngleRadian/2)));
            // Euh... je gère pas les sprites correctement, je règlerai ça plus tard
            List<string> skinName = new List<string>(1);
            skinName.Add("bullet001-1");
            //// Génération des nbTirs tirs
            //// Allocation des données
            //Vector2[] vect = new Vector2[shotNb];
            //Shot[] shot = new Shot[shotNb];
            //Sprite[] shotSprite = new Sprite[shotNb];
            //// Création des sprites et affectation des vitesses
            //for (uint i = 0; i<shotNb; i++)
            //{
            //    vect[i] = new Vector2 (shotNb-i)*debut+i*fin;
            //    shot[i] = new Shot(this.LuxGame, 1, null);
            //    shotSprite[i] = new Sprite(shot[i], skinName);
            //    shot[i].Speed = vect[i];
            //    shot[i].Position = this.Position;
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
                currentVector.X = (float)(Math.Cos(bigAngleRadian / (shotNb - 1)) * currentVector.X - Math.Sin(bigAngleRadian / (shotNb - 1)) * currentVector.Y);
                currentVector.Y = (float)(Math.Sin(bigAngleRadian / (shotNb - 1)) * currentVector.X + Math.Cos(bigAngleRadian / (shotNb - 1)) * currentVector.Y);
            }
            //    shot[i].Skin = shotSprite[i];
            //    //Ajout au jeu des éléments
            //    Game.Components.Add(shot[i]);
            //    Game.Components.Add(shotSprite[i]);
            //    shotSprite[i].SetAnimation("bullet001-1");
            //}
        }


        
    }
}
