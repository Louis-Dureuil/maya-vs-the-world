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
        public Enemy(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision,
            int shotNb, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.shotNb = shotNb;
            this.bulletText = this.Content.Load<Texture2D>("bullet001-1");
            this.shotHitbox = 8;
        }

        public Enemy(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, int shotNb,
            Sprite skin, Texture2D bulletText, int shotHitbox)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
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

        // MORCEAUX DE CODE A FACTORISER

        public void Create()
        {
            // Crée l'ennemi (duh)
            this.Enabled = true;
        }

        // Shoot tire une balle dans une direction
        public void Shoot(Vector2 direction)
        {
            int cnt = 0;
            bool fini = false;

            // On regarde tous les tirs dans la liste, ayant le contrôle dessus
            while (cnt < shotNb && !fini)
            {
                // Si l'un d'entre eux est sorti de l'écran, on peut le réutiliser
                if (shots[cnt].IsOutOfRange == true)
                {
                    shots[cnt].Speed = direction;
                    shots[cnt].Position = this.Position;
                    shots[cnt].Accel = new Vector2(0, 0);
                    shots[cnt].Shoot();
                    fini = true;
                }
                else
                {
                    cnt++;
                }
            }
            //if (cnt == shotNb)
            //{
            //    //On est à court de balles
            //    System.Console.WriteLine("ERREUR : Un ennemi n'a pas suffisamment de balles allouées");
            //}
        }

        // PatternShoot tire une salve de shotNb balles, espacées d'un angle entré en paramètre
        public void PatternShoot(Vector2 direction, int shotNb, float angleBtwShotsDegrees)
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
                Shoot(currentVector);
                currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
            }
        }

        // BigPatternShoot tire patNb salves de shotNb balles à des vitesses différentes
        // allant de initialSpeed à finalSpeed
        public void BigPatternShoot(Vector2 direction, int shotNb, float angleBtwShotsDegrees,
            int patNb, float initialSpeed, float finalSpeed)
        {
            if (patNb < 2)
            {
                return;
            }

            Vector2 currentDir = Vector2.Normalize(direction);
            Vector2 currentVector = initialSpeed * currentDir;
            for (int i = 0; i < patNb; i++)
            {
                PatternShoot(currentVector, shotNb, angleBtwShotsDegrees);
                currentVector += (finalSpeed - initialSpeed) / (patNb - 1) * currentDir;
            }
        }

        // SuperShoot est un Shoot un peu plus amélioré
        // Il faut entrer davantage de paramètres, la position et l'accélération
        public void SuperShoot(Vector2 position, Vector2 direction, Vector2 accel)
        {
            int cnt = 0;
            bool fini = false;

            while (cnt < shotNb && !fini)
            {
                if (shots[cnt].IsOutOfRange == true)
                {
                    shots[cnt].Speed = direction;
                    shots[cnt].Position = position;
                    shots[cnt].Accel = accel;
                    shots[cnt].Shoot();
                    fini = true;
                }
                else
                {
                    cnt++;
                }
            }

            //if (cnt == shotNb)
            //{
            //    //On est à court de balles
            //    System.Console.WriteLine("ERREUR : Un ennemi n'a pas suffisamment de balles allouées");
            //}
        }

        // RandomPatternShoot est un PatternShoot amélioré
        // On peut dévier la trajectoire initiale de balles avant le tir, d'un angle maxDeviationDegrees
        public void RandomPatternShoot(Vector2 position, Vector2 direction, Vector2 accel,
            float angleBtwShotsDegrees, int shotNb, int maxDeviationDegrees)
        {
            if (shotNb < 2)
            {
                return;
            }

            // Pour éviter d'effectuer des rotations sur un vecteur nul

            bool speedVectIsZero = false;
            bool accelVectIsZero = false;

            if (direction.X == 0 && direction.Y == 0)
            {
                speedVectIsZero = true;
            }
            if (accel.X == 0 && accel.Y == 0)
            {
                accelVectIsZero = true;
            }

            double bigAngleRadian = Math.PI / 180 * (shotNb - 1) * angleBtwShotsDegrees;
            Vector2 currentDirectionVector = direction;
            Vector2 currentAccelVector = accel;
            int deviationDegrees = Common.Rand.Next(-maxDeviationDegrees, maxDeviationDegrees);
            float deviationRadian = (float)Math.PI / 180 * deviationDegrees;
            if (!speedVectIsZero)
            {
                currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ(deviationRadian));
                currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ((float)bigAngleRadian / 2));
            }
            if (!accelVectIsZero)
            {
                currentAccelVector = Vector2.Transform(currentAccelVector, Matrix.CreateRotationZ(deviationRadian));
                currentAccelVector = Vector2.Transform(currentAccelVector, Matrix.CreateRotationZ((float)bigAngleRadian / 2));
            }
            for (int i = 0; i < shotNb; i++)
            {
                SuperShoot(position, currentDirectionVector, currentAccelVector);
                if (!speedVectIsZero)
                {
                    currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
                }
                if (!accelVectIsZero)
                {
                    currentAccelVector = Vector2.Transform(currentAccelVector, Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
                }
            }
        }

        // LongShoot tire shotNb balles, éloignées les unes des autres
        // par un vecteur speedAdd et accelAdd
        public void LongShoot(Vector2 position, Vector2 speed, Vector2 accel,
            int shotNb, Vector2 speedAdd, Vector2 accelAdd)
        {
            Vector2 speedVect = speed;
            Vector2 accelVect = accel;
            for (int i = 0; i < shotNb; i++)
            {
                SuperShoot(position, speedVect, accelVect);
                accelVect += accelAdd;
                speedVect += speedAdd;
            }
        }

        // LongPatternShoot est une salve de LongShots
        // stretch va allonger la salve horizontalement
        public void LongPatternShoot(Vector2 position, Vector2 speed, Vector2 accel, int shotNb,
            Vector2 speedAdd, Vector2 accelAdd, float angleBtwShotsDegrees,
            int patShotNb, int maxDeviationDegrees, float stretch)
        {
            if (shotNb < 2)
            {
                return;
            }

            // Pour éviter d'effectuer des rotations sur un vecteur nul

            bool speedVectIsZero = false;
            bool accelVectIsZero = false;

            if (speed.X == 0 && speed.Y == 0)
            {
                speedVectIsZero = true;
            }
            if (accel.X == 0 && accel.Y == 0)
            {
                accelVectIsZero = true;
            }

            double bigAngleRadian = Math.PI / 180 * (shotNb - 1) * angleBtwShotsDegrees;
            Vector2 currentDirectionVector = speed;
            Vector2 currentAccelVector = accel;
            int deviationDegrees = Common.Rand.Next(-maxDeviationDegrees, maxDeviationDegrees);
            float deviationRadian = (float)Math.PI / 180 * deviationDegrees;
            if (!speedVectIsZero)
            {
                currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ(deviationRadian));
                currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ((float)bigAngleRadian / 2));
            }
            if (!accelVectIsZero)
            {
                currentAccelVector = Vector2.Transform(currentAccelVector, Matrix.CreateRotationZ(deviationRadian));
                currentAccelVector = Vector2.Transform(currentAccelVector, Matrix.CreateRotationZ((float)bigAngleRadian / 2));
            }
            for (int i = 0; i < patShotNb; i++)
            {
                LongShoot(position, new Vector2(stretch*currentDirectionVector.X, currentDirectionVector.Y), currentAccelVector, shotNb, speedAdd, accelAdd);
                if (!speedVectIsZero)
                {
                    currentDirectionVector = Vector2.Transform(currentDirectionVector, Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
                }
                if (!accelVectIsZero)
                {
                    currentAccelVector = Vector2.Transform(currentAccelVector, Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
                }
            }
        }
    }
}
