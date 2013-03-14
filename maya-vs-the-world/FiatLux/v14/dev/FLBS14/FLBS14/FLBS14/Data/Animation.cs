using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Data
{
    [Serializable]
    public class Animation
    {
        public SerializableDictionary<string, double> Images;
        public bool IsLoop;
    }
}
