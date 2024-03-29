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
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = null)] HttpRequest req,
                                                    [DurableClient] IDurableOrchestrationClient starter,
                                                    ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var command = JsonConvert.DeserializeObject<RunWorkSimulationCommand>(requestBody);

            string instanceId = await starter.StartNewAsync("Workflow", null, command);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
