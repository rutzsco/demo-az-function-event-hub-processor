using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Demo.EventProcessor
{
    public static class EventProcessorActivity
    {
        [FunctionName("EventProcessorActivity")]
        public static async Task Run([EventHubTrigger("ingest-001", Connection = "IngestEventHubConnectionString")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();

            log.LogMetric("EventProcessorActivityBatchSize", events.Count(), new Dictionary<string, object> { { "RunId", Guid.NewGuid()} });
            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
