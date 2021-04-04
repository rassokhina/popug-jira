using System;

namespace Shared.Events
{
    public class UserCreatedEvent : Event
    {
        public string PublicId { get; set; }

        public string Role { get; set; }

        public string Username { get; set; }
    }
}
