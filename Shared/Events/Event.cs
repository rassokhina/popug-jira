using System;

namespace Shared.Events
{
    public abstract class Event
    {
        public Guid EventId { get; set; }

        public string EventName { get; set; }

        public DateTimeOffset EventTime { get; set; }

        public string EventProducer { get; set; }
    }
}
