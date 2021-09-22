using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Demo.EventProcessor
{
    public static class EventProcessorActivity
    {
        private static List<string> ignoreTags = new List<string>() { "Tag5", "Tag6" };

        [FunctionName("EventProcessorActivity")]
        public static async Task Run([EventHubTrigger("ingest-001", Connection = "IngestEventHubConnectionString")] EventData[] events, ILogger log)
        {
            log.LogMetric("EventProcessorActivityBatchSize", events.Count(), new Dictionary<string, object> { { "RunId", Guid.NewGuid()} });
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                //var telemetryModel = JsonSerializer.Deserialize<TelemetryModel>(messageBody);
                LogDiagnostics(eventData, messageBody, log);
            }
        }

        [FunctionName("EventProcessorActivity002")]
        public static async Task Run2([EventHubTrigger("ingest-002", Connection = "IngestEventHubConnectionString")] EventData[] events, ILogger log)
        {
            log.LogMetric("EventProcessorActivityBatchSize", events.Count(), new Dictionary<string, object> { { "RunId", Guid.NewGuid() } });
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                LogDiagnostics(eventData, messageBody, log);
            }
        }

        private static void LogDiagnostics(EventData eventData, string messageBodyString, ILogger log)
        {
            var sb = new StringBuilder();
            foreach (var properties in eventData.Properties)
            {
                sb.Append(properties.Key);
                sb.Append('=');
                sb.Append(properties.Value);
                sb.Append('|');
            }
            sb.Append("SystemProperties|");
            foreach (var properties in eventData.SystemProperties)
            {
                sb.Append(properties.Key);
                sb.Append('=');
                sb.Append(properties.Value);
                sb.Append('|');
            }
            log.LogInformation($"EventHubIngestionProcessor MessagePayload:  {messageBodyString}");
            log.LogInformation($"EventHubIngestionProcessor MessageProperties:  {sb}");
        }
    }
}
