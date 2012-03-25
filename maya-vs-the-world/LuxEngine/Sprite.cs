/* LuxEngine V0.2.20
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    [Serializable]
    public class InvalidAnimationException : Exception
    {
        public InvalidAnimationException() { }
        public InvalidAnimationException(string message) : base(message) { }
        public InvalidAnimationException(string message, Exception inner) : base(message, inner) { }
        protected InvalidAnimationException(
         System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// A component that handles animation. Inherits from Scene.
    /// </summary>
    public class Sprite : Scene
    {
        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height);
            }
        }

        Queue<Vector2> afterImages = new Queue<Vector2>();
        bool hasAfterImage = false;
        int afterImageNumbers = 20;
        public int AfterImageNumbers
        {
            get { return afterImageNumbers; }
            set
            {
                afterImageNumbers = value;
                while (afterImages.Count > value * 6)
                    afterImages.Dequeue();
            }
        }
        public bool HasAfterImage
        {
            get { return hasAfterImage; }
            set
            {
                hasAfterImage = value;
                if (!value)
                    afterImages.Clear();
            }
        }
        public Color AfterImageColor = Color.White;
        Effect afterImageEffect;

        Texture2D[] SpriteSheet;
        Dictionary<string, int> AnimationID = new Dictionary<string, int>();
        int width;
        int height;
        float scaleX = 1.0F;
        float scaleY = 1.0F;
        float rotation;
        public Color SpriteColor = Color.White;
        public SpriteEffects SpriteEffect = SpriteEffects.None;
        public Effect effect;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        int Animation = -1;
        Rectangle spriteRectangle;

        public float Width
        {
            get
            {
                return width * scaleX;
            }
            set { scaleX = value / (float)width; }
        }

        public float Height
        {
            get
            {
                return height * scaleY;
            }
            set { scaleY = value / (float)height; }
        }

        public Sprite(Scene parent, List<string> assetNames)
            : base(parent)
        {
            for (int i = 0; i < assetNames.Count; i++)
            {
                AnimationID.Add(assetNames[i], i);
            }
            SpriteSheet = new Texture2D[assetNames.Count];
            LoadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            afterImageEffect = Content.Load<Effect>("afterimageeffect");
            foreach (KeyValuePair<string, int> AnimationPair in AnimationID)
            {
                SpriteSheet[AnimationPair.Value] = this.Content.Load<Texture2D>(AnimationPair.Key);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }  

        public void SetAnimation(string AnimationName)
        {
            if (AnimationID.ContainsKey(AnimationName))
            {
                Animation = AnimationID[AnimationName];
                width = SpriteSheet[Animation].Width;
                height = SpriteSheet[Animation].Height;
                Width = width;
                Height = height;
            }
            else
                throw new InvalidAnimationException("The selected animation (" + AnimationName + ") is not available.");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (HasAfterImage)
            {
                afterImages.Enqueue(Position + new Vector2(1, 0));
                afterImages.Enqueue(Position + new Vector2(0, 1));
                afterImages.Enqueue(Position + new Vector2(1, 1));

                afterImages.Enqueue(Position + new Vector2(-1, 0));
                afterImages.Enqueue(Position + new Vector2(0, -1));
                afterImages.Enqueue(Position + new Vector2(-1, -1));
                if (afterImages.Count > 6 * afterImageNumbers)
                    for (int i = 0; i < 6; i++)
                        afterImages.Dequeue();
            }
            this.spriteRectangle.Width = (int)Width;
            this.spriteRectangle.Height = (int)Height;
            if (Animation >= 0)
            {
                if (HasAfterImage)
                {
                    float i = 0.0F;
                    foreach (Vector2 AfterImagePosition in afterImages)
                    {
                        i += 1.0F / afterImages.Count;
                        this.spriteRectangle.X = (int)(AfterImagePosition.X + parent.Position.X);
                        this.spriteRectangle.Y = (int)(AfterImagePosition.Y + parent.Position.Y);
                        afterImageEffect.Parameters["alpha"].SetValue(i/10);
                        LuxGame.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied,null, null, null, afterImageEffect);           
                        LuxGame.spriteBatch.Draw(SpriteSheet[Animation], spriteRectangle, null, AfterImageColor, Rotation, new Vector2(width / 2, height / 2), SpriteEffect, 0.0F);
                        LuxGame.spriteBatch.End();
                    }
                }
              LuxGame.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, effect);
                this.spriteRectangle.X = (int)positionWithParent.X;
                this.spriteRectangle.Y = (int)positionWithParent.Y;
                LuxGame.spriteBatch.Draw(SpriteSheet[Animation], spriteRectangle, null, SpriteColor, Rotation, new Vector2(width / 2, height / 2), SpriteEffect, 0.0F);
                LuxGame.spriteBatch.End();
            }
        }
    }
}
