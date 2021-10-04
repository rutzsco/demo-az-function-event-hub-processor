using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Demo.EventSender.Model;
using System.Diagnostics;

namespace Demo.EventEventSender
{
    public static class SendEventHubMessageEndpoint
    {
        [FunctionName("SendEventHubMessageEndpoint001")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [EventHub("ingest-001", Connection = "IngestEventHubConnectionString")] IAsyncCollector<string> outputEvents001, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            await outputEvents001.AddAsync(requestBody);
            return new OkObjectResult("OK");
        }

        [FunctionName("SendEventHubMessageEndpoint002")]
        public static async Task<IActionResult> Run002([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           [EventHub("ingest-002", Connection = "IngestEventHubConnectionString")] IAsyncCollector<string> outputEvents002, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            await outputEvents002.AddAsync(requestBody);
            return new OkObjectResult("OK");
        }

        [FunctionName("SendEventHubMessageScenarioEndpoint")]
        public static async Task<IActionResult> RunWithParamaters([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [EventHub("ingest-002", Connection = "IngestEventHubConnectionString")] IAsyncCollector<TelemetryModel> outputEvents002, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var command = JsonConvert.DeserializeObject<RunScenarioCommand>(requestBody);

            Stopwatch durationSW = new Stopwatch();
            durationSW.Start();
            while (durationSW.Elapsed < TimeSpan.FromSeconds(command.Scenario.DurationSeconds))
            {
                Stopwatch rateSW = new Stopwatch();
                rateSW.Start();
                while (rateSW.Elapsed < TimeSpan.FromSeconds(1))
                {
                    for (int x = 0; x < command.Scenario.RatePerSeconds; x++)
                    {
                        await outputEvents002.AddAsync(command.EventModel);
                    }          
                }
                rateSW.Stop();
                rateSW.Reset();
            }
            durationSW.Stop();

            return new OkObjectResult("OK");
        }
    }
}
