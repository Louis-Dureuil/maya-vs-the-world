using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup.Scenes
{
    public class TestScene : Scene
    {
        private Text text;

        public TestScene(LuxGame game)
            : base(game)
        {
            text =  new Text(this, "SchmupFont");
            Game.Components.Add(text);
        }

        public override void Initialize()
        {
            text.Message = "Hello World";
            text.Position = 400;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
