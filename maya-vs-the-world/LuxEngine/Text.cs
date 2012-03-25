/* LuxEngine V0.4.56
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
    /// A text object using a setting of font/size/color/effect
    /// </summary>
    public class Text : Scene
    {
        string message = "";
        float rotation;
        float scale = 1.0F;
        Vector2 size;
        Vector2 offset = Vector2.Zero;
        //SpriteFont spriteFont;
        Effect effect;
        Color color = Color.White;
        VerticalAlignment verticalAlignment;
        HorizontalAlignment horizontalAlignment;
        string spriteFontName;
        string effectName;

        protected override void LoadContent()
        {
            spriteFont = LuxGame.Content.Load<SpriteFont>(spriteFontName);
            if (effectName != null)
            effect = LuxGame.Content.Load<Effect>(effectName);
            base.LoadContent();
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set
            {
                verticalAlignment = value;
                CalculateOffset();
            }
        }
        public void CalculateOffset()
        {
            switch (horizontalAlignment)
            {
                case LuxEngine.HorizontalAlignment.Left:
                    offset.X = 0;
                    break;
                case LuxEngine.HorizontalAlignment.Center:
                    offset.X = Size.X / 2;
                    break;
                case LuxEngine.HorizontalAlignment.Right:
                    offset.X = Size.X;
                    break;
            }
            switch (verticalAlignment)
            {
                case LuxEngine.VerticalAlignment.Up:
                    offset.Y = 0;
                    break;
                case LuxEngine.VerticalAlignment.Center:
                    offset.Y = Size.Y / 2;
                    break;
                case LuxEngine.VerticalAlignment.Down:
                    offset.Y = Size.Y;
                    break;
            }
        }
        public HorizontalAlignment HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set
            {
                horizontalAlignment = value;
                CalculateOffset();
            }
        }

        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
            set
            {
                spriteFont = value;
                size = spriteFont.MeasureString(message);
                CalculateOffset();
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                size = spriteFont.MeasureString(message);
                CalculateOffset();
            }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }



        public Vector2 Size
        {
            get { return size; }
        }

        public Text(Scene parent, string spriteFontName, string effectName)
            : base(parent)
        {
            this.spriteFontName = spriteFontName;
            this.effectName = effectName;
        }

        public Text(Scene parent, string spriteFontName)
            : base(parent)
        {
            this.spriteFontName = spriteFontName;
        }
        public override void Draw(GameTime gameTime)
        {
            LuxGame.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, effect);
            //LuxGame.spriteBatch.DrawString(spriteFont, message, positionWithParent - offset, color, rotation, size / 2, scale, SpriteEffects.None, 0);
            LuxGame.spriteBatch.DrawString(spriteFont, message, positionWithParent, color, rotation,offset, scale, SpriteEffects.None, 0);
            LuxGame.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
