using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class BigLockShotPattern : ShotPattern
    {
        private int patternNb;
        private int shotNb;
        private int angleBtwShotsDegrees;
        private float initialSpeed;
        private float finalSpeed;
        private List<ShotPattern> spatterns = new List<ShotPattern>();
        private Texture2D bulletText;

        public BigLockShotPattern(LuxGame game, int shotNb, int angleBtwShotsDegrees, Texture2D bulletText, int patternNb, float initialSpeed, float finalSpeed)
            : base(game, shotNb, new Vector2(0,0), angleBtwShotsDegrees, bulletText)
        {
            this.patternNb = patternNb;
            this.initialSpeed = initialSpeed;
            this.finalSpeed = finalSpeed;
            this.shotNb = shotNb;
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
            this.bulletText = bulletText;
        }

        public override void Initialize()
        {
            base.Initialize();
            for (int i = 0; i < patternNb; i++)
            {
                ShotPattern spat = new ShotPattern(this.LuxGame, shotNb, new Vector2(0, 0), angleBtwShotsDegrees, bulletText);
                Game.Components.Add(spat);
                spatterns.Add(spat);
            }
        }

        public void Shoot(Vector2 direction)
        {
            Vector2 startVector = initialSpeed * Vector2.Normalize(direction);
            Vector2 currentVector = initialSpeed * Vector2.Normalize(direction);
            for (int i = 0; i< patternNb; i++)
            {
                spatterns[i].Direction=currentVector;
                spatterns[i].Position = this.Position;
                spatterns[i].Shoot();
                currentVector += ((finalSpeed - initialSpeed)/(patternNb-1)) * startVector;
            }
        }
    }
}
