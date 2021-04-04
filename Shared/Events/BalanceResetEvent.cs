using System;

namespace Shared.Events
{
    public class BalanceResetEvent: Event
    {
        public string UserId { get; set; }

        public Guid WalletId { get; set; }

        public DateTimeOffset Time { get; set; }

        public int BalanceChange { get; set; }
    }
}
