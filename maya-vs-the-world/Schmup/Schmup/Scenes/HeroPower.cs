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
        private int score;
        private int scoreTotal;
        private bool powerIsShot;

        public HeroPower(LuxGame game, int invincibleTimeMillisec, Sprite skin = null)
            : base(game, invincibleTimeMillisec, null)
        {
        }

        public override void Initialize()
        {
            score = 0;
            scoreTotal = 0;
            powerIsShot = false;
            base.Initialize();
            GoesThrough = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.isActionDone(Input.Action.Power, false))
            {
                powerIsShot = true;
                score = 0;
                Common.PowerHit = 0;
            }
            if (powerIsShot && IsOutOfRange)
            {
                powerIsShot = false;
                for (int i = 0; i < Common.PowerHit; i++)
                {
                    score += i + 1;
                }
                scoreTotal += score;
                Common.PowerHit = scoreTotal;
            }
            base.Update(gameTime);
        }
    }
}
