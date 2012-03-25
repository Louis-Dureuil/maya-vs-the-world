/* LuxEngine V0.2.16
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    /// <summary>
    /// A static class that contains the engine's shared constants and objects.
    /// </summary>
    public static class Shared
    {
        // Size of the windows borders.
        public const int FRAMESIZE = 6;
        // Message Lifetime, in seconds.
        public const float MESSAGELIFETIME = 5.0F;
        public static readonly Color SelectedColor = new Color(0, 0, 255);
        public static readonly Color DisabledColor = new Color(25, 25, 25);
        public const int MAPVERSION = 2;

        static float frameDuration;

        public static float FrameDuration
        {
            get { return frameDuration; }
        }

        public static Random Rand = new Random();

        internal static void Update(GameTime gameTime)
        {
            //Frame duration, in s (precision 10^(-3) s.)
            frameDuration = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0F;
        }
    }
}
