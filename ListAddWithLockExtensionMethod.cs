using System.Collections.Generic;

namespace DotNetTool
{
    public static class ListAddWithLockExtensionMethod
    {
        public static void AddWithLock<T>(this List<T> ls, T o)
        {
            lock ((ls as System.Collections.ICollection).SyncRoot)
            {
                ls.Add(o);
            }
        }
    }
}
