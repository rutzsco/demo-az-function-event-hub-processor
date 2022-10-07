using System.Collections.Generic;
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
        public static string Run([ActivityTrigger] int waitSeconds, ILogger log)
        {
            Thread.Sleep(waitSeconds * 1000);
            return $"OK";
        }

        [FunctionName("Workflow")]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            var command = context.GetInput<RunWorkSimulationCommand>();

            var tasks = new Task<string>[command.Count];
            for (int i = 0; i < command.Count; i++)
            {
                tasks[i] = context.CallActivityAsync<string>("DoWorkActivity", command.Duration);
            }
            await Task.WhenAll(tasks);
 
            return outputs;
        }
    }
}