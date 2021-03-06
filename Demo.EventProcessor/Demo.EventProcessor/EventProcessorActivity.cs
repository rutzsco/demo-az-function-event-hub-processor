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
        public static async Task Run([EventHubTrigger("ingest-001", Connection = "IngestEventHubConnectionString")] EventData[] events,
                                     [EventHub("output-001", Connection = "OutputEventHubConnectionString")] IAsyncCollector<string> outputEvents, ILogger log)
        {
            log.LogMetric("EventProcessorActivityBatchSize", events.Count(), new Dictionary<string, object> { { "RunId", Guid.NewGuid()} });
            foreach (EventData eventData in events)
            {
                var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var telemetryModel = JsonSerializer.Deserialize<TelemetryModel>(messageBody);
                var telemetryModelFiltered = new TelemetryModel() { Data = telemetryModel.Data.Where(t => !ignoreTags.Any(i => i == t.Name)) };
                await outputEvents.AddAsync(JsonSerializer.Serialize(telemetryModelFiltered));
            }
        }
    }
}
