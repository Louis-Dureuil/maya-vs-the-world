using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLBS14.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FLBS14.Manager
{
    public class SpriteManager
    {
        private List<Sprite> sprites = new List<Sprite>();
        private SpriteBatch spriteBatch;

        public SpriteManager(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public void Load(Sprite sprite)
        {
            sprites.Add(sprite);
        }

        public void Update(GameTime gameTime)
        {
            foreach (Sprite sprite in sprites)
            {
                if (sprite.CurrentAnimation != null)
                {
                    sprite.CurrentAnimation.Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (Sprite sprite in sprites)
            {
                if (sprite.CurrentAnimation == null)
                {
                    spriteBatch.DrawString(GameCommon.SpriteFont, "No Animation", new Vector2(300, 300), Color.Red);
                }
                else if (sprite.CurrentAnimation.CurrentImage == null)
                {
                    spriteBatch.DrawString(GameCommon.SpriteFont, "No Image", new Vector2(300, 300), Color.Orange);
                }
                else if (sprite.CurrentAnimation.CurrentImage.Texture == null)
                {
                    spriteBatch.DrawString(GameCommon.SpriteFont, "Loading...", new Vector2(300, 300), Color.Yellow);
                }
                else
                {
                    spriteBatch.Draw(sprite.CurrentAnimation.CurrentImage.Texture,
                        sprite.DestRectangle,
                        sprite.CurrentAnimation.CurrentImage.SrcRectangle,
                        sprite.Color,
                        sprite.Angle,
                        sprite.Origin,
                        sprite.Effect,
                        sprite.Z);
                }
            }
            spriteBatch.End();
        }
    }
}
