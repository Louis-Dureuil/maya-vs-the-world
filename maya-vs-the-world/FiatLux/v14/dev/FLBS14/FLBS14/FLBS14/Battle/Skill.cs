using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Battle
{
    public class Skill
    {
        #region private
        private string name;
        private float strength;
        private bool isMagic;
        #endregion

        public Skill(string name, float strength, bool isMagic)
        {
            this.name = name;
            this.strength = strength;
            this.isMagic = isMagic;
        }

        public string Name
        {
            get { return name; }
        }

        public float Strength
        {
            get { return strength; }
        }

        public bool IsMagic
        {
            get { return isMagic; }
        }
       
    }
}
