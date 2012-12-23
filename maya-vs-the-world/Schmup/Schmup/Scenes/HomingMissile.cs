using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class HomingMissile : Shot
    {
        // rotation est l'angle limite (en radians) de rotation du tir par frame
        private float rotation;

        public HomingMissile(LuxGame game, int invincibleTimeMillisec, bool isAGoodShot, float rotation, int hitbox, World world, Sprite skin = null)
            : base(game, invincibleTimeMillisec, isAGoodShot, world, hitbox, null)
        {
            this.rotation = rotation;
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

        // AngleBetweenRadians donne l'angle entre deux vecteurs
        // Sans souci de continuité
        public float AngleBetweenRadians(Vector2 vect1, Vector2 vect2)
        {
            float angle = AngleRadian(vect1) - AngleRadian(vect2);
            if (angle > (float)Math.PI)
            {
                angle -= 2*(float)Math.PI;
            }
            else if (angle < -(float)Math.PI)
            {
                angle += 2*(float)Math.PI;
            }
            return angle;
        }

        public override void Update(GameTime gameTime)
        {
            float angle = AngleBetweenRadians(Speed, - Position);
            if (angle > rotation)
            {
                angle = rotation;
            }
            else if (angle < -rotation)
            {
                angle = -rotation;
            }
            Speed = Vector2.Transform(Speed, Matrix.CreateRotationZ(angle));
            base.Update(gameTime);
        }
    }
}
