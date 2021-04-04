using System;

namespace Shared.Events
{
    public class BalancePaidEvent: Event
    {
        public string UserId { get; set; }

        public string WalletId { get; set; }

        public DateTimeOffset Time { get; set; }

        public int Paid { get; set; }
    }
}
