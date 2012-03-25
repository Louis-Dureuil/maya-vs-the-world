using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
namespace FiatLux.Scenes
{
    public class ClassMenuWindow : Window
    {
        public ClassMenuWindow(Scene parent, Logic.Character chara)
            : base(parent, new Rectangle(0, 0, 400, 400))
        {

        }
    }
}
