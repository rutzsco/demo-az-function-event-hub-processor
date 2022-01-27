using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Demo.EventProcessor
{
    public static class EventProcessorActivity001
    {
        private static List<string> ignoreTags = new List<string>() { "Tag5", "Tag6" };

        [FunctionName("EventProcessorActivity001")]
        public static async Task Run([EventHubTrigger("ingest-001", Connection = "IngestEventHubConnectionString")] EventData[] events, [DurableClient] IDurableClient context, ILogger log, PartitionContext partitionContext)
        {
            log.LogMetric("EventProcessorActivityBatchSize", events.Count(), new Dictionary<string, object> { { "PartitionId", partitionContext.PartitionId } });
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var telemetryModel = JsonSerializer.Deserialize<TelemetryModel>(messageBody);
                Diagnostics.Log(eventData, messageBody, log, partitionContext);

                Logic.Execute(telemetryModel);
            }
        }
    }
}
