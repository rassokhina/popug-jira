using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskTracker.Core.Entities
{
    public class Popug
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public PopugRole Role { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
