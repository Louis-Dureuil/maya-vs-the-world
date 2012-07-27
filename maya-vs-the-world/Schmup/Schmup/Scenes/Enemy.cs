using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuxEngine;
using Microsoft.Xna.Framework;

namespace Schmup
{
    class Enemy : Character
    {
        // private uint weapon;
        // weapon a changer

        public Enemy(LuxGame game, int life, int takenDamageCollision, int givenDamageCollision, Sprite skin)
            : base(game, life, takenDamageCollision, givenDamageCollision, skin)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            //this.Enabled = false;
        }

        //public void Create()
        //{
        //    this.Enabled = true;
        //}
    }
}
