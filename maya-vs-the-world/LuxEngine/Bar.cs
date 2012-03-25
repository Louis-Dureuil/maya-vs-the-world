/* LuxEngine V0.2.17
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
    /// <summary>
    /// A component that shows the rate between a value and a max value. Inherits from Scene.
    /// </summary>
    public class Bar : Scene
    {
        float timer=0.0F;
        int maxValue;
        int val;
        public Vector2 Size;
        public Color ValueColor = Color.White;
        public Color BackgroundColor = new Color(0,0,0,128);
        Texture2D barTexture;
        public int MaxValue { get { return maxValue; } set { maxValue = Math.Max(0, value); } }
        public int Value { get { return val; } set { val = Math.Max(0, Math.Min(MaxValue, value)); } }

        Effect barEffect;

        public Bar(Scene parent, Vector2 Size)
            : base(parent)
        {
            this.Size = Size;
        }

        protected override void LoadContent()
        {
            barTexture = Game.Content.Load<Texture2D>("Bar");
            barEffect = Game.Content.Load<Effect>("barEffect");
            base.LoadContent();
        }

        public Bar(LuxGame game, Vector2 Size)
            : base(game)
        {
            this.Size = Size;
        }

        public override void Initialize()
        {
            base.Initialize();
            //DrawOrder++;
        }

        public override void Draw(GameTime gameTime)
        {
            
            if (barEffect != null)
            {
                //Frame duration, in s (precision 10^(-3) s.)
                float FrameDuration = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0F;
                if (timer >= 1.0F)
                    timer = 0.0F;
                else
                {
                    timer += FrameDuration * (MaxValue - Value) / (MaxValue) + FrameDuration / 5;
                    barEffect.Parameters["timer"].SetValue(timer);
                }
                barEffect.Parameters["state"].SetValue((float)Value / (float)MaxValue);             
            }
            LuxGame.spriteBatch.Begin();
            LuxGame.spriteBatch.Draw(barTexture, new Rectangle((int)(positionWithParent.X), (int)(positionWithParent.Y), (int)Size.X, (int)Size.Y), BackgroundColor);

            LuxGame.spriteBatch.End();
           
            LuxGame.spriteBatch.Begin(SpriteSortMode.Immediate,null,null,null,null,null);
            LuxGame.spriteBatch.Draw(barTexture, new Rectangle((int)(positionWithParent.X), (int)(positionWithParent.Y), (int)(((float)Value / (float)MaxValue) * Size.X), (int)Size.Y), ValueColor);
            LuxGame.spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Destroy()
        {
            barTexture = null;
            base.Destroy();
        }
    }
}
