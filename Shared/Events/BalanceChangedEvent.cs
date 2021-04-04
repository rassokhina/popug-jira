using System;

namespace Shared.Events
{
    public class BalanceChangedEvent: Event
    {
        public string UserId { get; set; }

        public string WalletId { get; set; }

        public DateTimeOffset Time { get; set; }

        public int BalanceChange { get; set; }
    }
}
