using System.Collections.Generic;

namespace Assets.TD.scripts.Utils.Extensions
{
    public static class MyExtensions
    {
        public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enu)
        {
            foreach (T obj in enu)
                queue.Enqueue(obj);
        }
    }
}