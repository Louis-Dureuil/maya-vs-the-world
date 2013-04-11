using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class GenericEnemy : Enemy
    {
        private Vector2 speed;
        private Vector2 accel;

        public GenericEnemy(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, skin)
        {
            speed = new Vector2(0, 0);
        }

        // AngleRadian donne l'angle que fait un vecteur
        public float AngleRadian(Vector2 vect)
        {
            // a perfectionner?
            float angle;
            if (vect.Y == 0)
            {
                if (vect.X < 0)
                {
                    angle = 0;
                }
                else
                {
                    angle = (float)Math.PI;
                }
            }
            else if (vect.Y > 0)
            {
                angle = (float)Math.Atan((double)vect.X / vect.Y);
            }
            else
            {
                angle = (float)(Math.PI + Math.Atan((double)vect.X / vect.Y));
            }
            return angle;
        }

        public override void Initialize()
        {
            accel = new Vector2(0, 0);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Modification de l'accélération :
            // On veut que l'accélération soit la plus aléatoire possible
            // Tout en gardant l'ennemi sur l'écran
            // TODO : Changer les 30, 770 etc en pourcentage!
            if (Position.X < 30)
            {
                accel.X += (float)2;
            }
            else if (Position.X > 770)
            {
                accel.X -= (float)2;
            }
            else
            {
                accel.X += (float)(Common.Rand.NextDouble() - 0.5);
            }
            if (Position.Y < 30)
            {
                accel.Y += (float)2;
            }
            else if (Position.Y > 450)
            {
                accel.Y -= (float)2;
            }
            else
            {
                accel.Y += (float)(Common.Rand.NextDouble() - 0.5);
            }
            speed += accel / 40;
            Position += speed;
            if (speed.Length() > 5)
            {
                accel = -accel/2;
            }
            if (speed.Length() > 15)
            {
                speed = new Vector2(0, 0);
                accel = new Vector2(0, 0);
            }

            if (Position.X < 0)
            {
                Position.X = 0;
            }
            if (Position.Y < 0)
            {
                Position.Y = 0;
            }
            if (Position.X > 800)
            {
                Position.X = 800;
            }
            if (Position.Y > 480)
            {
                Position.Y = 480;
            }

            base.Update(gameTime);
        }
    }
}
