using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.EventProcessor
{
    public static class StartWorkEndpoint
    {
        [FunctionName("StartWorkEndpoint")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    [DurableClient] IDurableOrchestrationClient starter,
                                                    ILogger log)
        {

            var duration = ParseDuration(req);

            string instanceId = await starter.StartNewAsync("Workflow", null, duration);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        private static int ParseDuration(HttpRequest req)
        {
            var duration = req.Query["duration"];
            if (string.IsNullOrEmpty(duration))
                return 300;
            try
            {
                int result = Int32.Parse(duration);
                return result;
            }
            catch (FormatException)
            {
                return 60;
            }
        }
    }
}
