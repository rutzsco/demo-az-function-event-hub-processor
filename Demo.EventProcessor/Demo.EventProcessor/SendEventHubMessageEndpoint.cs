using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.EventProcessor
{
    public static class SendEventHubMessageEndpoint
    {
        [FunctionName("SendEventHubMessageEndpoint")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, 
            [EventHub("ingest-001", Connection = "IngestEventHubConnectionString")] IAsyncCollector<string> outputEvents, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            await outputEvents.AddAsync(requestBody);
            return new OkObjectResult("OK");
        }
    }
}
