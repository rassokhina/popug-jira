using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Core.Entities
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; } 

        [Required]
        public string Description { get; set; }

        [Required]
        public int Price { get; set; }

        public TaskStatus Status { get; set; }

        public DateTimeOffset? Finished { get; set; }

        public Guid? PopugId { get; set; }

        public virtual Popug Popug { get; set; }
    }
}
