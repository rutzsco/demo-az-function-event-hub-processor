using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.EventEventSender
{
    public static class SendEventHubMessageEndpoint
    {
        [FunctionName("SendEventHubMessageEndpoint")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{hub}")] HttpRequest req,  string hub,
            [EventHub("ingest-001", Connection = "IngestEventHubConnectionString")] IAsyncCollector<string> outputEvents001,
            [EventHub("ingest-002", Connection = "IngestEventHubConnectionString")] IAsyncCollector<string> outputEvents002, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (hub == "002")
            {
                await outputEvents002.AddAsync(requestBody);
            }
            else
                await outputEvents001.AddAsync(requestBody);

            return new OkObjectResult("OK");
        }
    }
}
