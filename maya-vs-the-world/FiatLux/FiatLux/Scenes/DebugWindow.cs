using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace FiatLux.Scenes
{
    public class DebugWindow : TextWindow
    {
        int index = 0;
        float[] FPS = new float[60];
        Vector2[] Positions = new Vector2[5];
        int PositionIndex = 0;
        public DebugWindow(LuxGame game)
            : base(game, new Rectangle(0, 0, 100, 100))
        {
            this.Visible = false;
            for (int i = 0; i < FPS.Length; i++)
            {
                FPS[i] = 0;
            }
            this.DrawOrder = 9999;
        }
        public override void Initialize()
        {
            Positions[0] = new Vector2(0, 0);
            Positions[1] = new Vector2(0, 0);
            Positions[2] = new Vector2(LuxGame.GraphicsDevice.Viewport.Width - 100 - Shared.FRAMESIZE, 0);
            Positions[3] = new Vector2(LuxGame.GraphicsDevice.Viewport.Width - 100 - Shared.FRAMESIZE, LuxGame.GraphicsDevice.Viewport.Height - 100 - Shared.FRAMESIZE);
            Positions[4] = new Vector2(0, LuxGame.GraphicsDevice.Viewport.Height - 100 - Shared.FRAMESIZE);
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            
            float FrameDuration = Shared.FrameDuration;
            FPS[index] = 1 / FrameDuration;
            index = (index + 1) % FPS.Length;
            FPS[index] = 0;
            
            float avgFPS = 0;
            for (int i = 0; i < FPS.Length; i++)
            {
                if (i != index)
                    avgFPS += FPS[i];
            }
            avgFPS /= FPS.Length - 1;
            Text = "Avg. FPS:\n" + avgFPS.ToString();
            if (index != 0)
            {
                Text += "Current FPS:\n" + FPS[(index - 1) % FPS.Length].ToString();
            }
            else
                Text += "Current FPS:\n" + FPS[FPS.Length - 1].ToString();
            

            base.Update(gameTime);
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.isActionDone(Input.Action.Debug, false))
            {
            
                PositionIndex = (PositionIndex + 1) % Positions.Length;
                Position = Positions[PositionIndex];
                if (PositionIndex == 0)
                    Visible = false;
                else
                    Visible = true;
            }
        }
    }
}
