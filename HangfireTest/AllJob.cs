using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;

namespace HangfireTest
{
    public class AllJob
    {
        public async Task RunAllJob(PerformContext context, CancellationToken cancellationToken)
        {
            var count = 2_000;
            var progressBar = context.WriteProgressBar();

            var c = 0;

            for (var i = 0; i < count; ++i)
            {
                BackgroundJob.Enqueue<EmptyJob>(x => x.RunEmptyJob());
                progressBar.SetValue(100.0 * i / count);
                cancellationToken.ThrowIfCancellationRequested();
            }

            // Enumerable.Range(0, count).AsParallel().ForAll(_ =>
            // {
            //     BackgroundJob.Enqueue<EmptyJob>(x => x.RunEmptyJob());
            //     progressBar.SetValue(100.0 * c++ / count);
            //     cancellationToken.ThrowIfCancellationRequested();
            // });

            // var queue = new ConcurrentQueue<int>(Enumerable.Range(0, count));
            // var tasks = new List<Task>();
            // for (var i = 0; i < 10; ++i)
            // {
            //     var task = Task.Factory.StartNew(_ =>
            //     {
            //         while (queue.TryDequeue(out var _))
            //         {
            //             BackgroundJob.Enqueue<EmptyJob>(x => x.RunEmptyJob());
            //             progressBar.SetValue(100.0 * c++ / count);
            //         }
            //     }, TaskCreationOptions.LongRunning);
            //     tasks.Add(task);
            // }
            // await Task.WhenAll(tasks);

            context.WriteLine("Finished");
            await Task.CompletedTask;
        }
    }
}