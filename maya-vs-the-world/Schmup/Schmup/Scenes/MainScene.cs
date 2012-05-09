using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;

namespace Schmup
{
    class MainScene : Scene
    {
        public MainScene(LuxGame game)
            : base(game)
        {
            List<string> skinName = new List<string>(1);
            skinName.Add("carre");
            Hero hero = new Hero(game, 1, 0, 0, new Sprite(this, skinName));
        }
    }
}
