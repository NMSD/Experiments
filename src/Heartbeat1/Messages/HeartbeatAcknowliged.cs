using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace Heartbeat.Messages
{
    public class HeartbeatAcknowliged
    {
        public DateTime Timestamp { get; set; }
    }
}
