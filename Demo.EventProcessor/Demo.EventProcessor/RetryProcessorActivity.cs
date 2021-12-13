using System;
using System.Text.Json;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Demo.EventProcessor
{
    public static class RetryProcessorActivity
    {
        [FunctionName("RetryProcessorActivity")]
        public static void Run([QueueTrigger("retry-events", Connection = "AzureStorageFailureQueueConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            var telemetryModel = JsonSerializer.Deserialize<TelemetryModel>(myQueueItem);
            Logic.Execute(telemetryModel);
        }
    }
}
