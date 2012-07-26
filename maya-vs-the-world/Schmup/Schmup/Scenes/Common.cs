using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schmup
{
    class Common
    {
        private static Random rand = new Random();
        public static Random Rand
        {
        get { return rand; }
        }
    }
}
