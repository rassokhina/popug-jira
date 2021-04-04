using System;

namespace Shared.Events
{
    public class TaskFinishedEvent : Event
    {
        public string TaskId { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
