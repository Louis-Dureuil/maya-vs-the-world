using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Schmup
{
    class Hero : Character
    {
        private uint mana;
        private uint currentWeapon;
        private uint speed;
        private uint currentPower;
        private List<uint> weaponList;
        private List<uint> speedList;
        private List<uint> powerList;
        private uint invincibleTime;


        public override void Update(GameTime gameTime)
        {
            if (Input.isActionDone(Input.Action.Up,true))
            {
                Position.Y -= speed;
            }
            else if (Input.isActionDone(Input.Action.Down, true))
            {
                Position.Y += speed;
            }
            if (Input.isActionDone(Input.Action.Left, true))
            {
                Position.X -= speed;
            }
            else if (Input.isActionDone(Input.Action.Right, true))
            {
                Position.X += speed;
            }
        }
    }
}
