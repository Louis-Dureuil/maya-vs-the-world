/* LuxEngine V0.2.18
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LuxEngine
{
    /// <summary>
    /// Overrides the Game class to provide a common spritebatch for all components.
    /// Also provide common loading abilities for other components.
    /// </summary>
    public class LuxGame : Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        //The audio objects costs over 40 MB overall!
        public AudioEngine engine;
        public SoundBank soundBank;
        public WaveBank waveBank;
        private ContentManager contentManager;
        

        public LuxGame()
        {
            graphics = new GraphicsDeviceManager(this);
          
            Content.RootDirectory = "Content";
        }

        public ContentManager GlobalContentManager
        {
            get { return contentManager; }
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            graphics.GraphicsProfile = GraphicsProfile.Reach;
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            contentManager = new ContentManager(this.Services, "Content");
            
            //Initialize Input
            Input.Initialize();
          //uncomment when the .wav are available for compilation (on pengouin-fixe)
            //engine = new AudioEngine("Content\\fiatluxsound.xgs");
            //soundBank = new SoundBank(engine, "Content\\Sound Bank.xsb");
            //waveBank = new WaveBank(engine, "Content\\Sound.xwb");

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update(gameTime);
            Shared.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

    }
}
