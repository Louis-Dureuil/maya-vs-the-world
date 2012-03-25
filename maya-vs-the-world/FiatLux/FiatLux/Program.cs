/* Fiat Lux V0.1.2
 * by Louis "Lars Rosenblau" Dureuil
 * All rights reserved 2006-2011.
 */

using System;
using LuxEngine;

namespace FiatLux
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (LuxGame game = new LuxGame())
            {
                
#if DEBUG
                game.Components.Add(new Scenes.DebugWindow(game));
#endif
                //game.IsFixedTimeStep = false;
                game.Components.Add(new Scenes.DemoTitleScene(game));
                game.Run();
            }
        }
    }
#endif
}

