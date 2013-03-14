using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Battle
{
    public class Stat
    {
        public string Name { get { return name; } }
        public string ShortName { get { return sname; } }
        public int Val { get { return val; } set { val = Math.Max(0, Math.Min(max, value)); } }
        public int MaxVal { get { return max; } set { max = value; } }

        public Stat(string sname, string name)
        {
            this.name = name;
            this.sname = sname;
            this.max = GameCommon.Rand.Next(1, 100);
            this.val = max;
        }

        #region private
        private string name;
        private string sname;
        private int val;
        private int max;
        #endregion

        public static explicit operator int(Stat from)
        {
            return from.val; 
        }
    }
}
