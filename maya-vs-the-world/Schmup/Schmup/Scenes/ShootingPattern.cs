using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class ShootingPattern : Scene
    {
        protected ShotPool shots;
        protected Enemy enemy;

        public ShootingPattern (LuxGame game, ShotPool shots, Enemy enemy)
            : base(game)
        {
            this.shots = shots;
            this.enemy = enemy;
        }

        public override void Update(GameTime gameTime)
        {
            shots.Check();
            base.Update(gameTime);
        }
    }
}
