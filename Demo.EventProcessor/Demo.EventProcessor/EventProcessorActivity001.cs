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
    public static class EventProcessorActivity001
    {
        private static List<string> ignoreTags = new List<string>() { "Tag5", "Tag6" };

        [FunctionName("EventProcessorActivity001")]
        public static async Task Run([EventHubTrigger("ingest-001", Connection = "IngestEventHubConnectionString")] EventData[] events, ILogger log, PartitionContext partitionContext)
        {
            log.LogMetric("EventProcessorActivityBatchSize001", events.Count(), new Dictionary<string, object> { { "PartitionId", partitionContext.PartitionId } });
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.EventBody);
                var telemetryModel = JsonSerializer.Deserialize<TelemetryModel>(messageBody);
                Diagnostics.Log(eventData, messageBody, log, partitionContext,"001");

                Logic.Execute(telemetryModel);
            }
        }
    }
}
