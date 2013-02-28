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
        private List<ShotPull> shotsOfType;

        public List<ShotPull> ShotsOfType
        {
            get
            {
                return shotsOfType;
            }
        }

        public Enemy(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, skin)
        {
            shotsOfType = new List<ShotPull>();
        }

        public Enemy(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision,
            List<ShotPull> shotsOfType, Sprite skin)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.shotsOfType = shotsOfType;
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = false;
        }

        public void Activate()
        {
            Enabled = true;
        }

        public override void Die()
        {
            World.Enemies.Remove(this);
            base.Die();
        }

        public override void Update(GameTime gameTime)
        {
            // Check des balles

            foreach (ShotPull shots in shotsOfType)
            {
                shots.Check();
            }
            base.Update(gameTime);
        }


        // TODO : Traiter tout ça
        // Aussi vérifier si parfois une recopie ne ferait pas de mal!!

        // Shoot tire une balle de type typeIndex dans une direction direction
        public void Shoot(int typeIndex, Vector2 direction)
        {
            shotsOfType[typeIndex].Shoot(Position.X, Position.Y, direction.X, direction.Y, 0, 0, false, false, false);
        }

        // PatternShoot tire une salve de shotNb balles, espacées d'un angle entré en paramètre
        public void PatternShoot(int typeIndex, Vector2 direction, int shotNb, float angleBtwShotsDegrees)
        {
            if (shotNb < 1)
            {
                return;
            }

            double bigAngleRadian = Math.PI / 180 * (shotNb - 1) * angleBtwShotsDegrees;
            Vector2 currentVector = direction;
            currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float)bigAngleRadian / 2));

            for (int i = 0; i < shotNb; i++)
            {
                Shoot(typeIndex, currentVector);
                currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
            }
        }

        // BigPatternShoot tire patNb salves de shotNb balles à des vitesses différentes
        // allant de initialSpeed à finalSpeed
        public void BigPatternShoot(int typeIndex, Vector2 direction, int shotNb, float angleBtwShotsDegrees,
            int patNb, float initialSpeed, float finalSpeed)
        {
            if (patNb < 1)
            {
                return;
            }

            if (patNb == 1)
            {
                PatternShoot(typeIndex, direction, shotNb, angleBtwShotsDegrees);
                return;
            }

            Vector2 currentDir = Vector2.Normalize(direction);
            Vector2 currentVector = initialSpeed * currentDir;
            for (int i = 0; i < patNb; i++)
            {
                PatternShoot(typeIndex, currentVector, shotNb, angleBtwShotsDegrees);
                currentVector += (finalSpeed - initialSpeed) / (patNb - 1) * currentDir;
            }
        }

        // SuperShoot est un Shoot un peu plus amélioré
        // Il faut entrer davantage de paramètres, la position et l'accélération
        public void SuperShoot(int typeIndex, Vector2 position, Vector2 direction, Vector2 accel)
        {
            shotsOfType[typeIndex].Shoot(position.X, position.Y, direction.X, direction.Y, accel.X, accel.Y,
                false, false, false);
        }

        // RandomPatternShoot est un PatternShoot amélioré
        // On peut dévier la trajectoire initiale de balles avant le tir, d'un angle maxDeviationDegrees
        public void RandomPatternShoot(int typeIndex, Vector2 position, Vector2 direction, Vector2 accel,
            float angleBtwShotsDegrees, int shotNb, int maxDeviationDegrees)
        {
            if (shotNb < 1)
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
                SuperShoot(typeIndex, position, currentDirectionVector, currentAccelVector);
                if (!speedVectIsZero)
                {
                    currentDirectionVector = Vector2.Transform(currentDirectionVector,
                        Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
                }
                if (!accelVectIsZero)
                {
                    currentAccelVector = Vector2.Transform(currentAccelVector,
                        Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
                }
            }
        }

        // LongShoot tire shotNb balles, éloignées les unes des autres
        // par un vecteur speedAdd et accelAdd
        public void LongShoot(int typeIndex, Vector2 position, Vector2 speed, Vector2 accel,
            int shotNb, Vector2 speedAdd, Vector2 accelAdd)
        {
            Vector2 speedVect = speed;
            Vector2 accelVect = accel;
            for (int i = 0; i < shotNb; i++)
            {
                SuperShoot(typeIndex, position, speedVect, accelVect);
                accelVect += accelAdd;
                speedVect += speedAdd;
            }
        }

        // LongPatternShoot est une salve de LongShots
        // stretch va allonger la salve horizontalement
        public void LongPatternShoot(int typeIndex, Vector2 position, Vector2 speed, Vector2 accel,
            int shotNb, Vector2 speedAdd, Vector2 accelAdd, float angleBtwShotsDegrees,
            int patShotNb, int maxDeviationDegrees, float stretch)
        {
            if (shotNb < 1)
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
                LongShoot(typeIndex, position, new Vector2(stretch * currentDirectionVector.X, currentDirectionVector.Y),
                    currentAccelVector, shotNb, speedAdd, accelAdd);
                if (!speedVectIsZero)
                {
                    currentDirectionVector = Vector2.Transform(currentDirectionVector,
                        Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
                }
                if (!accelVectIsZero)
                {
                    currentAccelVector = Vector2.Transform(currentAccelVector,
                        Matrix.CreateRotationZ((float)-bigAngleRadian / (shotNb - 1)));
                }
            }
        }

    }
}
