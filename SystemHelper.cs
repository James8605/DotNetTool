using System;
using System.Diagnostics;

namespace XGS.James.Tool
{
    public class SystemHelper
    {
        public static int GetMemoryUsedInGB()
        {
            using Process proc = Process.GetCurrentProcess();
            var mem = proc.PrivateMemorySize64;

            return Convert.ToInt32(mem / (1024 * 1024 * 1024));
        }
    }
}
