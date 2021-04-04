using System;

namespace Shared.Events
{
    public class TaskUpdatedEvent: Event
    {
        public string TaskId { get; set; }

        public int Price { get; set; }
    }
}
