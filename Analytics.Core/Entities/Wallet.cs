using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Analytics.Core.Entities
{
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string PublicId { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        public int Balance { get; set; }

        [Required]
        public DateTimeOffset ChangeTime { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

    }
}
