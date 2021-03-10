using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace BezelTools
{
    /// <summary>
    /// Thread-related methods
    /// </summary>
    public static class ThreadUtils
    {
        private static readonly List<Thread> threads = new List<Thread>();

        /// <summary>
        /// Runs threads on the specified files
        /// </summary>
        /// <param name="threadsNb">The number of threads to run</param>
        /// <param name="inputFiles">The input files collection</param>
        /// <param name="threadMethod">The method executed by the thread</param>
        public static void RunThreadsOnFiles(int threadsNb, IEnumerable<string> inputFiles, Action<string> threadMethod)
        {
            // get files to process
            var files = new ConcurrentQueue<string>(inputFiles);

            // run threads
            for (int i = 0; i < threadsNb; i++)
            {
                var t = new Thread(() =>
                {
                    while (files.TryDequeue(out var f))
                    {
                        threadMethod(f);
                    }
                });
                threads.Add(t);
                t.Start();
            }

            // wait for all threads to finish
            for (int t = 0; t < threads.Count; t++)
            {
                threads[t].Join();
            }

            // clear up threads list
            threads.Clear();
        }
    }
}
