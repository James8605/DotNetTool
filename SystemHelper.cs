using System;
using System.Diagnostics;

namespace DotNetTool
{
    public class SystemHelper
    {
        public static int GetMemoryUsedInGB()
        {
            using Process proc = Process.GetCurrentProcess();
            var mem = proc.PrivateMemorySize64;

            return Convert.ToInt32(mem / (1024 * 1024 * 1024));
        }

        public static void ForceGC()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        public static T RunWithTimeMeasured<T>(Func<object[], T> func, object[] param, Type classType, string log)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(classType);

            var sw = Stopwatch.StartNew();

            T ret = func(param);

            logger.Info($"{log}耗时{Convert.ToInt32(sw.ElapsedMilliseconds)}毫秒");

            return ret;
        }
    }
}
