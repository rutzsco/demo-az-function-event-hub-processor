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
using Microsoft.Extensions.Logging;

namespace Demo.EventProcessor
{
    public static class EventProcessorActivity
    {
        private static List<string> ignoreTags = new List<string>() { "Tag5", "Tag6" };

        [FunctionName("EventProcessorActivity")]
        public static async Task Run([EventHubTrigger("ingest-001", Connection = "IngestEventHubConnectionString")] EventData[] events, ILogger log, PartitionContext context)
        {
            log.LogMetric("EventProcessorActivityBatchSize", events.Count(), new Dictionary<string, object> { { "PartitionId", context.PartitionId } });
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var telemetryModel = JsonSerializer.Deserialize<TelemetryModel>(messageBody);
                LogDiagnostics(eventData, messageBody, log, context);
                Thread.Sleep(telemetryModel.DelayMS);
            }
        }

        [FunctionName("EventProcessorActivity002")]
        public static async Task Run2([EventHubTrigger("ingest-002", Connection = "IngestEventHubConnectionString")] EventData[] events, ILogger log, PartitionContext context)
        {
            log.LogMetric("EventProcessorActivityBatchSize", events.Count(), new Dictionary<string, object> { { "RunId", Guid.NewGuid() } });
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var telemetryModel = JsonSerializer.Deserialize<TelemetryModel>(messageBody);
                LogDiagnostics(eventData, messageBody, log, context);
                Thread.Sleep(telemetryModel.DelayMS);
            }
        }

 
        private static void LogDiagnostics(EventData eventData, string messageBodyString, ILogger log, PartitionContext context)
        {
            var messageSequence = eventData.SystemProperties.SequenceNumber;
            var lastEnqueuedSequence = context.RuntimeInformation.LastSequenceNumber;
            var sequenceDifference = lastEnqueuedSequence - messageSequence;
            log.LogMetric("EventProcessorActivityPartitionSequenceLag", sequenceDifference, new Dictionary<string, object> { { "PartitionId", context.PartitionId } });

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
