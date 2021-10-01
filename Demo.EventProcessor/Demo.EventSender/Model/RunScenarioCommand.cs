using Demo.EventEventSender;

using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.EventSender.Model
{
    public class RunScenarioCommand
    {
        public TelemetryModel EventModel { get; set; }

        public Scenario Scenario { get; set; }
    }
}
