using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace FiatLux.Scenes
{
    public class BattlePostScreenScene : Scene
    {
        BattleScene battleParent;
        List<BattleExpGauge> ExpGauges;
        public BattlePostScreenScene(BattleScene parent)
            : base(parent)
        {
            ExpGauges = parent.ExpGauges;
            battleParent = parent;
            this.Enabled = true;
        }
        public override void Initialize()
        {
            base.Initialize();
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Destroy()
        {
            base.Destroy();
         }
    }
}
