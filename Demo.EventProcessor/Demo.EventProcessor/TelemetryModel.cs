using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.EventProcessor
{

//    {
//   "data":[
//      {
//         "name":"Tag1",
//         "value":"Value1 "
//      },
//      {
//         "name":"Tag1",
//         "value":"Value1"
//      },
//      {
//         "name":"Tag2",
//         "value":"Value1"
//      },
//      {
//         "name":"Tag3",
//         "value":"Value1"
//      },
//      {
//         "name":"Tag4",
//         "value":"Value1 "
//      },
//      {
//         "name":"Tag5",
//         "value":"Value1"
//      }
//   ]
//}

    public class TelemetryModel
    {
        public int DelayMS { get; set; }

        public IEnumerable<Tag> Data { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
