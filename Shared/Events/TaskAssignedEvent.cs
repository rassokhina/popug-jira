using System;

namespace Shared.Events
{
    public class TaskAssignedEvent
    {
        public string UserId { get; set; }

        public Guid TaskId { get; set; }

        public DateTimeOffset Time { get; set; }
    }
    
}
