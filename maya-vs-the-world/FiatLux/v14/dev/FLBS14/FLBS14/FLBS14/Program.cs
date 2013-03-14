using System;

namespace FLBS14
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Battle.Battle game = new Battle.Battle())
            {
                game.Run();
            }
        }
    }
#endif
}

