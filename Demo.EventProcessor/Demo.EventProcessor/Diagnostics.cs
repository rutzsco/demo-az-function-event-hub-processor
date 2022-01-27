using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.EventProcessor
{
    internal class Diagnostics
    {
        internal static void Log(EventData eventData, string messageBodyString, ILogger log, PartitionContext partitionContext)
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
