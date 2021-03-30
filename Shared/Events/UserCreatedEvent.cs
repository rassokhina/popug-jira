using System;

namespace Shared.Events
{
    public class UserCreatedEvent
    {
        public string Id { get; set; }

        public string Role { get; set; }

        public string Username { get; set; }
    }
}
