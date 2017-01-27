using System;

namespace Sokoban
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameMgr(1024, 768))
            {
                game.Run();
            }
        }
    }
#endif
}
