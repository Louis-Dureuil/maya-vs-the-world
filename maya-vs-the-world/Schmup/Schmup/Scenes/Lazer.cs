using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class Lazer : Shot
    {
        // CLASSE NON VALIDEE!

        //hitbox
        private Vector2 move;
        private int lazerNb;

        public Lazer(LuxGame game, World world, int invincibleTimeMillisec, Vector2 move, int lazerNb)
            : base(game, world, invincibleTimeMillisec)
        {
            this.move = move;
            this.lazerNb = lazerNb;
        }
        private Lazer lazer;

        public override void Initialize()
        {
            base.Initialize();
            //if (lazerNb > 0)
            //{
            //    lazer = new Lazer(this.LuxGame, world, 0, move, lazerNb - 1);
            //    lazer.Position = this.Position + move;
            //    Sprite skin = new Sprite(lazer, new List<string>() { "lazermoche" });
            //    skin.SetAnimation("lazermoche");
            //    lazer.Skin = skin;
            //    Game.Components.Add(lazer);
            //    Game.Components.Add(skin);
            //}
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //if (lazerNb > 10)
            //{
            //    lazer.Position = this.Position + move;
            //}
        }
    }
}
