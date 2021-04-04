using System;

namespace Accounting.Core.Dto
{
    public class WalletAuditDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public int MoneyChange { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
