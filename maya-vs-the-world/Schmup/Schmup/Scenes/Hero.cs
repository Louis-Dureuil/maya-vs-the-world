using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private KeyboardState _keyboardState;


        public override void Update(GameTime gameTime)
        {
            if (_keyboardState.IsKeyDown(Keys.Z) || _keyboardState.IsKeyDown(Keys.z))
            {
                Position.Y -= speed;
            }
            else if (_keyboardState.IsKeyDown(Keys.S) ||_keyboardState.IsKeyDown(Keys.s))
            {
                _zozorPosition.Y++;
            }

            if (_keyboardState.IsKeyDown(Keys.Q) || _keyboardState.IsKeyDown(Keys.q))
            {
                _zozorPosition.X++;
            }
            if (_keyboardState.IsKeyDown(Keys.D) || _keyboardState.IsKeyDown(Keys.d)
            {
                _zozorPosition.X--;
            }
        }
    }
}
