using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14
{
    class GameConstants
    {
        public const int MaxAp = 1000;
        public const int MinAp = 100;
        public const int BasAp = 300;
        public const int ModAp = 200;
        public static readonly Dictionary<string, float> Speed = new Dictionary<string,float>()
        {
            { "Ended", 1000 }, 
            { "Normal", 10 },
            { "Inactive", 300},
            { "Interactive", 1F}
        };
    }
}
