using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Demo.EventProcessor
{
    public static class FailureTrackingActivity
    {

        [JsonObject(MemberSerialization.OptIn)]
        public class FailureTracker
        {
            [JsonProperty("value")]
            public int Value { get; set; }

            public void TrackFailure()
            {
                this.Value += 1;

                if (Value >= 3)
                {

                    // TODO: Logic to stop funciton.
                    //Entity.Current.StartNewOrchestration("StopEventProcessingActivity", Entity.Current.EntityId);
                }
            }

            public Task Reset()
            {
                this.Value = 0;
                return Task.CompletedTask;
            }

            public Task<int> Get()
            {
                return Task.FromResult(this.Value);
            }

            public void Delete()
            {
                Entity.Current.DeleteState();
            }

            [FunctionName("FailureTracker")]
            public static Task Run([EntityTrigger] IDurableEntityContext ctx)
                => ctx.DispatchAsync<FailureTracker>();
        }
    }
}