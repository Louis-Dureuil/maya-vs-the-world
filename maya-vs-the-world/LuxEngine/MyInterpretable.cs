using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using LuxEngine.Commands;

namespace LuxEngine
{
    public class MyInterpretable : Interpretable
    {
        SayCommand sc;
        SayCommand sca; 

        public MyInterpretable(Scene parent)
            : base(parent)
        {
            sc = new SayCommand(parent, "Coucou, tu veux voir ma ville ?");
            sca = new SayCommand(parent, "Non merci.");
        }

        public override void Initialize()
        {
            base.Initialize();
            Game.Components.Add(sc);
            Game.Components.Add(sca);
            sca.Enabled = false;
            sca.Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!sc.IsFinished)
            {
                return;
            }
            sca.Enabled = true;
            sca.Visible = true;
            if (!sca.IsFinished)
            {
                return;
            }
        }

    }
}
