using System;
using System.Collections.Generic;
using System.Text;

namespace MMOLoginServer
{
    public static class Debug
    {
        public static bool enable = true;
        public static void Log(string msg)
        {
#if DEBUG           
            if (enable)
                Console.WriteLine(msg);
#endif
        }
        public static void Log(string msg, params object[] param)
        {
#if DEBUG           
            if (enable)
                Console.WriteLine(msg, param);
#endif
        }
    }
}
