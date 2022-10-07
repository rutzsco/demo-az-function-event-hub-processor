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
    public static class SendQueueMessage
    {
        [FunctionName("SendQueueMessage")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [ServiceBus("telemetry001", Connection = "ServiceBusConnectionString")] IAsyncCollector<dynamic> output,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

  
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            await output.AddAsync(requestBody);
           
            return new OkObjectResult("OK");
        }
    }
}
