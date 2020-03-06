using System;
using System.Collections.Generic;
using System.Text;

namespace MMOGameServer
{
    public class Debug
    {
        public static bool enabled = true;
        public static void Log(object msg)
        {
            if (enabled)
                Console.WriteLine(msg);
        }
    }
}
