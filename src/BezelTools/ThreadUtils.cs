using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BezelTools
{
    /// <summary>
    /// Thread-related methods
    /// </summary>
    public static class ThreadUtils
    {
        private static readonly List<Thread> threads = new();

        /// <summary>
        /// The delegate to run a method asynchronously
        /// </summary>
        /// <param name="threadsNb">The number of threads.</param>
        /// <param name="inputFiles">The input files.</param>
        /// <param name="threadMethod">The thread method to run on the files.</param>
        public delegate void RunAsyncDelegate(int threadsNb, IEnumerable<string> inputFiles, Action<string> threadMethod);

        /// <summary>
        /// Gets or sets the method to run methods asynchronously.
        /// </summary>
        public static RunAsyncDelegate RunAsync { get; set; } = RunThreadsOnFiles;

        /// <summary>
        /// Runs threads on the specified files
        /// </summary>
        /// <param name="threadsNb">The number of threads to run</param>
        /// <param name="inputFiles">The input files collection</param>
        /// <param name="threadMethod">The method executed by the thread</param>
        private static void RunThreadsOnFiles(int threadsNb, IEnumerable<string> inputFiles, Action<string> threadMethod)
        {
            // get files to process
            var files = new ConcurrentQueue<string>(inputFiles.OrderBy(f => f));

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