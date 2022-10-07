using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Demo.MessageProcessor
{
    public class MessageProcessorActivity
    {
        [FunctionName("MessageProcessorActivity")]
        public void Run([ServiceBusTrigger("telemetry001", Connection = "ServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
