using System;

namespace Shared.Events
{
    public class TaskCreatedEvent: Event
    {
        public string TaskId { get; set; }

        public DateTimeOffset Created { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }
    }
}
