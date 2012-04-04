using System;

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
            using (ShmupGame game = new ShmupGame())
            {
                //game.Components.Add(new Scenes.DemoTitleScene(game));
                game.Run();
            }
        }
    }
#endif
}
