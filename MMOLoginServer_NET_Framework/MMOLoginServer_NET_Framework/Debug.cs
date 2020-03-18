using System;
using System.Collections.Generic;
using System.Text;

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
