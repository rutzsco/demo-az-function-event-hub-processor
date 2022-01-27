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
    public static class SendEventHubMessageScenarioEndpoint001
    {
        [FunctionName("SendEventHubMessageScenarioEndpoint001")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [EventHub("ingest-001", Connection = "IngestEventHubConnectionString")] IAsyncCollector<TelemetryModel> outputEvents002, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var command = JsonConvert.DeserializeObject<RunScenarioCommand>(requestBody);

            int messageCount = 0;
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
                        messageCount++;
                        await outputEvents002.AddAsync(command.EventModel);
                    }          
                }
                rateSW.Stop();
                rateSW.Reset();
            }
            durationSW.Stop();
            return new OkObjectResult($"Events sent: {messageCount}");
        }
    }
}
