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
using System.Net.Http;

namespace Demo.EventEventSender
{
    public class SendHttpMessageScenarioEndpoint001
    {
        private readonly HttpClient _client;

        public SendHttpMessageScenarioEndpoint001(IHttpClientFactory httpClientFactory)
        {
            this._client = httpClientFactory.CreateClient();
        }


        [FunctionName("SendHttpMessageScenarioEndpoint001")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
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
                    var response = _client.GetAsync(command.Scenario.TargetUrl);
                    //log.LogInformation($"Invoked http request with response code: {response.StatusCode}");
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
