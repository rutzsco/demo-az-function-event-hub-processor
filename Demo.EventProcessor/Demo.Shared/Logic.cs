using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Demo.EventProcessor
{
    public static class Logic
    {
        public static void Execute(TelemetryModel telemetryModel)
        {
            Thread.Sleep(telemetryModel.DelayMS);
        }
    }
}
