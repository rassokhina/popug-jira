using System;
using System.Collections.Generic;

using Accounting.Core.Dto;

namespace Accounting.Client.Models
{
    public class WalletModel
    {
        public int EarnedByManagement { get; set; }

        public IEnumerable<WalletAuditDto> WalletAudits { get; set; }
    }
}
