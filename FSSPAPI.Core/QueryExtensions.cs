using System;
using System.Collections.Generic;
using System.Text;

namespace FSSPAPI.Core
{
    internal static class QueryExtensions
    {

        //https://stackoverflow.com/a/33551927
        public static IEnumerable<T> DequeueChunk<T>(this Queue<T> queue,int chunkSize)
        {
            for(int i=0;i<chunkSize && queue.Count > 0; i++)
            {
                yield return queue.Dequeue();
            }
        }
    }
}
