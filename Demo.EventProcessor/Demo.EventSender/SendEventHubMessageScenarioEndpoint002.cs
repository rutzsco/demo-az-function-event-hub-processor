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
using System.Threading;

namespace Demo.EventEventSender
{
    public static class SendEventHubMessageScenarioEndpoint002
    {
        [FunctionName("SendEventHubMessageScenarioEndpoint002")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [EventHub("ingest-002", Connection = "IngestEventHubConnectionString")] IAsyncCollector<TelemetryModel> outputEvents002, ILogger log)
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
                for (int x = 0; x < command.Scenario.RatePerSeconds; x++)
                {
                    messageCount++;
                    await outputEvents002.AddAsync(command.EventModel);
                }

                while (rateSW.Elapsed < TimeSpan.FromSeconds(1))
                {
                    Thread.Sleep(10);
                }
                rateSW.Stop();
                rateSW.Reset();
            }
            durationSW.Stop();
            return new OkObjectResult($"Events sent: {messageCount}");
        }
    }
}
