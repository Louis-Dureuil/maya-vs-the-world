using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schmup.Scenes
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
    }
}
