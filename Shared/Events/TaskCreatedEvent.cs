using System;

namespace Shared.Events
{
    public class TaskCreatedEvent
    {
        public Guid Id { get; set; }

        public DateTimeOffset Created { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }
    }
}
