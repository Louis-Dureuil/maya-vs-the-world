using System;
using LuxEngine;

namespace Schmup
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
                game.Components.Add(new Scenes.TestScene(game));
                game.Run();
            }
        }
    }
#endif
}
