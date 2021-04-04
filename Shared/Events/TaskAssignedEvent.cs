using System;

namespace Shared.Events
{
    public class TaskAssignedEvent: Event
    {
        public string UserId { get; set; }

        public string TaskId { get; set; }

        public DateTimeOffset Time { get; set; }
    }
    
}
