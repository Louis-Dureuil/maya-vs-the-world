using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLBS14.Data
{
    [Serializable]
    public class Sprite
    {
        public SerializableDictionary<string, string> Animations;
        public double X;
        public double Y;
        public double W;
        public double H;
        public float Z;
        public float Angle;
        public byte A;
        public byte B;
        public byte G;
        public byte R;
        public float OriginX;
        public float OriginY;
        public bool IsVerticallyMirrored;
        public bool IsHorizontallyMirrored;
    }
}
