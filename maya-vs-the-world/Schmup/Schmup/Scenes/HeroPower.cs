using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schmup
{
    class HeroPower : HeroShot
    {
        // CLASSE NON VALIDEE!

        private int score;
        private int scoreTotal;
        private bool powerIsShot;

        public HeroPower(LuxGame game, World world, int invincibleTimeMillisec, int damage, Sprite skin = null)
            : base(game, world, invincibleTimeMillisec, true, damage, null)
        {
        }

        public override void Initialize()
        {
            score = 0;
            scoreTotal = 0;
            powerIsShot = false;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.isActionDone(Input.Action.Power, false))
            {
                powerIsShot = true;
                score = 0;
            }
            if (powerIsShot && IsOutOfRange)
            {
                powerIsShot = false;
                for (int i = 0; i < 3; i++)
                {
                    score += i + 1;
                }
                scoreTotal += score;
            }
            base.Update(gameTime);
        }
    }
}
