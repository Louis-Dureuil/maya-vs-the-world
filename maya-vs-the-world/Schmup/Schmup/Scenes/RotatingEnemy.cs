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
        private double elapsed;
        private int shotNb;
        private float angleBtwShotsDegrees;
        private ShotPattern spat;
        private int shotCnt;
        private double waitTimeSec;
        private Texture2D bulletTexture;
        private int patShotNb;
        private int patAngleBtwShotsDegrees;


        private Vector2 currentVector;

        private List<ShotPattern> spats = new List<ShotPattern>();

        public RotatingEnemy(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin, int shotNb, float angleBtwShotsDegrees, ShotPattern spat, double waitTimeSec, int patShotNb, int patAngleBtwShotsDegrees)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.shotNb = shotNb;
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
            this.spat = spat;
            this.waitTimeSec = waitTimeSec;
            this.bulletTexture = this.Content.Load<Texture2D>("bullet001-1");
            this.patAngleBtwShotsDegrees = patAngleBtwShotsDegrees;
            this.patShotNb = patShotNb;
        }

        public RotatingEnemy(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin, int shotNb, float angleBtwShotsDegrees, ShotPattern spat, double waitTimeSec, int patShotNb, int patAngleBtwShotsDegrees, Texture2D bulletTexture)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {
            this.shotNb = shotNb;
            this.angleBtwShotsDegrees = angleBtwShotsDegrees;
            this.spat = spat;
            this.waitTimeSec = waitTimeSec;
            this.bulletTexture = bulletTexture;
            this.patAngleBtwShotsDegrees = patAngleBtwShotsDegrees;
            this.patShotNb = patShotNb;
        }

        public override void Initialize()
        {
            base.Initialize();
            shotCnt = 0;
            currentVector = spat.Direction;
            for (int i = 0; i < shotNb; i++)
            {
                ShotPattern spattern = new ShotPattern(this.LuxGame, patShotNb, spat.Direction, patAngleBtwShotsDegrees, bulletTexture);
                Game.Components.Add(spattern);
                spats.Add(spattern);
            }
        }

        public void Shoot()
        {
            spats[shotCnt].Position = this.Position;
            spats[shotCnt].Direction = currentVector; 
            spats[shotCnt].Shoot();
            shotCnt++;
            currentVector = Vector2.Transform(currentVector, Matrix.CreateRotationZ((float)Math.PI/180*angleBtwShotsDegrees));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            elapsed += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= waitTimeSec && shotCnt < shotNb)
            {
                elapsed = 0; // On réinitialise le compteur
                Shoot();
                shotCnt++;
            }
            if (gameTime.TotalGameTime.Milliseconds.Equals(waitTimeSec) && shotCnt < shotNb)
            {
                Shoot();
            }
        }
    }
}
