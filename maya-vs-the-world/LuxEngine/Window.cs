/* LuxEngine V0.1.12
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
    /// A component with a frame that contains other components (bars, text, ...).Inherits from Scene.
    /// </summary>
    public class Window : Scene
    {
        public Rectangle WindowRectangle;

        public bool WithFrame = true;

        Texture2D windowSkin;
        Vector2 Velocity = Vector2.Zero;
        Vector2? Destination;

        public Color BackgroundColor = Color.White;

        public Window(LuxGame game, Rectangle rect)
            : base(game)
        {
            WindowRectangle = rect;
            Position = new Vector2(rect.X, rect.Y);
        }

        public Window(Scene parent, Rectangle rect)
            : base(parent)
        {
            WindowRectangle = rect;
            Position = new Vector2(rect.X, rect.Y);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteFont = this.Game.Content.Load<SpriteFont>("FontTest");
            //spriteFont = Content.Load<SpriteFont>("DefaultSpriteFont");
            windowSkin = this.Game.Content.Load<Texture2D>("WindowSkinBack1");
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Dispose(bool disposing)
        {            
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Position = Position + Velocity;
            WindowRectangle.X = (int)positionWithParent.X;
            WindowRectangle.Y = (int)positionWithParent.Y;
         
            const int BORDERPIXEL = 40;
            LuxGame.spriteBatch.Begin();
            if (WithFrame)
            {
                // Draw Background
                LuxGame.spriteBatch.Draw(windowSkin, new Rectangle(WindowRectangle.X + Shared.FRAMESIZE, WindowRectangle.Y + Shared.FRAMESIZE, WindowRectangle.Width - Shared.FRAMESIZE, WindowRectangle.Height - Shared.FRAMESIZE), new Rectangle(BORDERPIXEL, BORDERPIXEL, 1024 - BORDERPIXEL * 2, 768 - BORDERPIXEL * 2), BackgroundColor);
                // Draw Frame

                LuxGame.spriteBatch.Draw(windowSkin, new Rectangle(WindowRectangle.X, WindowRectangle.Y, Shared.FRAMESIZE, Shared.FRAMESIZE), new Rectangle(0, 0, BORDERPIXEL, BORDERPIXEL), Color.White);
                LuxGame.spriteBatch.Draw(windowSkin, new Rectangle(WindowRectangle.X + WindowRectangle.Width, WindowRectangle.Y, Shared.FRAMESIZE, Shared.FRAMESIZE), new Rectangle(windowSkin.Width - BORDERPIXEL, 0, BORDERPIXEL, BORDERPIXEL), Color.White);
                LuxGame.spriteBatch.Draw(windowSkin, new Rectangle(WindowRectangle.X, WindowRectangle.Y + WindowRectangle.Height, Shared.FRAMESIZE, Shared.FRAMESIZE), new Rectangle(0, windowSkin.Height - BORDERPIXEL, BORDERPIXEL, BORDERPIXEL), Color.White);
                LuxGame.spriteBatch.Draw(windowSkin, new Rectangle(WindowRectangle.X + WindowRectangle.Width, WindowRectangle.Y + WindowRectangle.Height, Shared.FRAMESIZE, Shared.FRAMESIZE), new Rectangle(windowSkin.Width - BORDERPIXEL, windowSkin.Height - BORDERPIXEL, BORDERPIXEL, BORDERPIXEL), Color.White);

                LuxGame.spriteBatch.Draw(windowSkin, new Rectangle(WindowRectangle.X + Shared.FRAMESIZE, WindowRectangle.Y, WindowRectangle.Width - Shared.FRAMESIZE, Shared.FRAMESIZE), new Rectangle(BORDERPIXEL, 0, windowSkin.Width - BORDERPIXEL * 2, BORDERPIXEL), Color.White);
                LuxGame.spriteBatch.Draw(windowSkin, new Rectangle(WindowRectangle.X, WindowRectangle.Y + Shared.FRAMESIZE, Shared.FRAMESIZE, WindowRectangle.Height - Shared.FRAMESIZE), new Rectangle(0, BORDERPIXEL, BORDERPIXEL, windowSkin.Height - BORDERPIXEL * 2), Color.White);
                LuxGame.spriteBatch.Draw(windowSkin, new Rectangle(WindowRectangle.X + WindowRectangle.Width, WindowRectangle.Y + Shared.FRAMESIZE, Shared.FRAMESIZE, WindowRectangle.Height - Shared.FRAMESIZE), new Rectangle(windowSkin.Width - BORDERPIXEL, BORDERPIXEL, BORDERPIXEL, windowSkin.Height - BORDERPIXEL * 2), Color.White);
                LuxGame.spriteBatch.Draw(windowSkin, new Rectangle(WindowRectangle.X + Shared.FRAMESIZE, WindowRectangle.Y + WindowRectangle.Height, WindowRectangle.Width - Shared.FRAMESIZE, Shared.FRAMESIZE), new Rectangle(BORDERPIXEL, windowSkin.Height - BORDERPIXEL, windowSkin.Width - BORDERPIXEL * 2, BORDERPIXEL), Color.White);
            }
            LuxGame.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
