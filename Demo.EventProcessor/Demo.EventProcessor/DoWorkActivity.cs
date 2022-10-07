using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Demo.EventProcessor
{
    public static class DoWorkActivity
    {
        [FunctionName("DoWorkActivity")]
        public static string Run([ActivityTrigger] DoWorkModel model, ILogger log)
        {
            log.LogInformation($"Executing Activity: {model.Id}");
            Thread.Sleep(model.DurationSeconds * 1000);
            return $"OK";
        }

        [FunctionName("Workflow")]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            var command = context.GetInput<RunWorkSimulationCommand>();

            Stopwatch durationSW = new Stopwatch();
            durationSW.Start();

            var tasks = new Task<string>[command.Count];
            for (int i = 0; i < command.Count; i++)
            {
                tasks[i] = context.CallActivityAsync<string>("DoWorkActivity", new DoWorkModel(i, command.Duration));
            }
            await Task.WhenAll(tasks);

            durationSW.Stop();
            outputs.Add($"Processed {command.Count} events in {durationSW.ElapsedMilliseconds} milliseconds");
            return outputs;
        }
    }
}