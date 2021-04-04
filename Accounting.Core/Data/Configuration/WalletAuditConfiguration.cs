using Accounting.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Core.Data.Configuration
{
    internal sealed class WalletAuditConfiguration :  IEntityTypeConfiguration<WalletAudit>
    {
        public void Configure(EntityTypeBuilder<WalletAudit> builder)
        {
        }
    }
}
