using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Analytics.Client.Entities
{
    public class Task
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; } 

        [Required]
        public string Description { get; set; }

        public TaskStatus Status { get; set; }

        public DateTimeOffset? Finished { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
