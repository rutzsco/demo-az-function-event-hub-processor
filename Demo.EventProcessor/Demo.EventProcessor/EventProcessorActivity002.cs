using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Demo.EventProcessor
{
    public static class EventProcessorActivity002
    {
        private static List<string> ignoreTags = new List<string>() { "Tag5", "Tag6" };

        [FunctionName("EventProcessorActivity002")]
        public static async Task Run([EventHubTrigger("ingest-002", Connection = "IngestEventHubConnectionString")] EventData[] events, ILogger log, PartitionContext partitionContext)
        {
            log.LogMetric("EventProcessorActivityBatchSize002", events.Count(), new Dictionary<string, object> { { "PartitionId", partitionContext.PartitionId } });
            foreach (EventData eventData in events)
            {
                var telemetryModel = JsonSerializer.Deserialize<TelemetryModel>(eventData.EventBody);
                //Diagnostics.Log(eventData, messageBody, log, partitionContext,"002");

                Logic.Execute(telemetryModel);
            }
        }
    }
}
