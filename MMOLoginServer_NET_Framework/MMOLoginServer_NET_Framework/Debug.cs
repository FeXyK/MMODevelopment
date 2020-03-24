using System;

namespace MMOLoginServer
{
    public class Debug
    {
        internal static bool enable;

        public static void Log(string msg)
        {
            if (enable)
                Console.WriteLine(msg);
        }
    }
}
