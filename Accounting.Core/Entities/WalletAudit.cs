using System;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Core.Entities
{
    public class WalletAudit
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string PublicId { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public Guid WalletId { get; set; }

        public string Description { get; set; }

        public int Debit { get; set; }

        public int Credit { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public virtual Wallet Wallet { get; set; }
    }
}
