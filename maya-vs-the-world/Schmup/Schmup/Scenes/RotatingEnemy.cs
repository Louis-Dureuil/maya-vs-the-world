using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class RotatingEnemy : Enemy
    {
        // CLASSE NON VALIDEE!

        private double elapsed;
        private int shotNb;
        private float angleBtwShotsDegrees;
        private Vector2 direction;
        private int shotCnt;
        private double waitTimeSec;
        private int patShotNb;
        private int patAngleBtwShotsDegrees;


        private Vector2 currentVector;

        public RotatingEnemy(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin, int shotNb, float angleBtwShotsDegrees, Vector2 direction, double waitTimeSec, int patShotNb, int patAngleBtwShotsDegrees, int bulletNumber)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, bulletNumber, skin)
        {
            this.shotNb = shotNb;
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
            this.direction = direction;
            this.waitTimeSec = waitTimeSec;
            this.patAngleBtwShotsDegrees = patAngleBtwShotsDegrees;
            this.patShotNb = patShotNb;
        }

        public RotatingEnemy(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin, int shotNb, float angleBtwShotsDegrees, Vector2 direction, double waitTimeSec, int patShotNb, int patAngleBtwShotsDegrees, Texture2D bulletTexture, int bulletNumber, int shotHitbox)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, bulletNumber, skin, bulletTexture, shotHitbox, 2)
        {
            this.shotNb = shotNb;
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
            this.direction = direction;
            this.waitTimeSec = waitTimeSec;
            this.patAngleBtwShotsDegrees = patAngleBtwShotsDegrees;
            this.patShotNb = patShotNb;
        }

        public RotatingEnemy(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin, int shotNb, float angleBtwShotsDegrees, Vector2 direction, double waitTimeSec, int patShotNb, int patAngleBtwShotsDegrees, Texture2D bulletTexture, int shotHitbox)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, shotNb * patShotNb, skin, bulletTexture, shotHitbox, 2)
        {
            this.shotNb = shotNb;
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
            this.direction = direction;
            this.waitTimeSec = waitTimeSec;
            this.patAngleBtwShotsDegrees = patAngleBtwShotsDegrees;
            this.patShotNb = patShotNb;
        }

        public RotatingEnemy(LuxGame game, World world, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin, int shotNb, float angleBtwShotsDegrees, Vector2 direction, double waitTimeSec, int patShotNb, int patAngleBtwShotsDegrees)
            : base(game, world, life, takenDamageCollision, givenDamageCollision, shotNb * patShotNb, skin)
        {
            this.shotNb = shotNb;
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
            this.direction = direction;
            this.waitTimeSec = waitTimeSec;
            this.patAngleBtwShotsDegrees = patAngleBtwShotsDegrees;
            this.patShotNb = patShotNb;
        }

        public override void Initialize()
        {
            base.Initialize();
            shotCnt = 0;
            currentVector = direction;
        }

        public void Reset()
        {
            elapsed = 0;
            shotCnt = 0;
            Enabled = false;
            currentVector = direction;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= waitTimeSec && shotCnt < shotNb)
            {
                elapsed = 0; // On réinitialise le compteur
                PatternShoot(currentVector, patShotNb, patAngleBtwShotsDegrees);
                currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float)Math.PI / 180 * angleBtwShotsDegrees));
                shotCnt++;
            }
        }
    }
}
