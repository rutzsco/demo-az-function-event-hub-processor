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
    public static class EventProcessorActivity
    {
        private static List<string> ignoreTags = new List<string>() { "Tag5", "Tag6" };

        [FunctionName("EventProcessorActivity")]
        public static async Task Run([EventHubTrigger("ingest-001", Connection = "IngestEventHubConnectionString")] EventData[] events, [DurableClient] IDurableClient context, ILogger log, PartitionContext partitionContext)
        {
            log.LogMetric("EventProcessorActivityBatchSize", events.Count(), new Dictionary<string, object> { { "PartitionId", partitionContext.PartitionId } });
            foreach (EventData eventData in events)
            {
                try
                {
                    var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var telemetryModel = JsonSerializer.Deserialize<TelemetryModel>(messageBody);
                    LogDiagnostics(eventData, messageBody, log, partitionContext);

                    Logic.Execute(telemetryModel);
                }
                catch
                {
                    var entityId = new EntityId("FailureTracker", "001");
                    await context.SignalEntityAsync(entityId, "TrackFailure");
                }
            }
        }

        private static void LogDiagnostics(EventData eventData, string messageBodyString, ILogger log, PartitionContext partitionContext)
        {
            var messageSequence = eventData.SystemProperties.SequenceNumber;
            var lastEnqueuedSequence = partitionContext.RuntimeInformation.LastSequenceNumber;
            var sequenceDifference = lastEnqueuedSequence - messageSequence;
            log.LogMetric("EventProcessorActivityPartitionSequenceLag", sequenceDifference, new Dictionary<string, object> { { "PartitionId", partitionContext.PartitionId } });

            var sb = new StringBuilder();    
            foreach (var properties in eventData.Properties)
            {
                sb.Append(properties.Key);
                sb.Append('=');
                sb.Append(properties.Value);
                sb.Append('|');
            }
            sb.Append("SystemProperties|");
            sb.Append("PartitionKey=");
            sb.Append(eventData.SystemProperties.PartitionKey);
            foreach (var properties in eventData.SystemProperties)
            {
                sb.Append(properties.Key);
                sb.Append('=');
                sb.Append(properties.Value);
                sb.Append('|');
            }
            log.LogDebug($"EventHubIngestionProcessor MessagePayload:  {messageBodyString}");
            log.LogDebug($"EventHubIngestionProcessor MessageProperties:  {sb}");
        }
    }
}
