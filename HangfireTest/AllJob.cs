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

            for (var i = 0; i < count; ++i)
            {
                BackgroundJob.Enqueue<EmptyJob>(x => x.RunEmptyJob());
                progressBar.SetValue(100.0 * i / count);
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            // parallel:
            // Enumerable.Range(0, count).AsParallel().ForAll(_ =>
            // {
            //     BackgroundJob.Enqueue<EmptyJob>(x => x.RunEmptyJob());
            //     progressBar.SetValue(100.0 * c++ / count);
            //     cancellationToken.ThrowIfCancellationRequested();
            // });

            context.WriteLine("Finished");
            await Task.CompletedTask;
        }
    }
}