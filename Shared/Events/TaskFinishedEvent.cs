using System;

namespace Shared.Events
{
    public class TaskFinishedEvent
    {
        public Guid TaskId { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
