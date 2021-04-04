using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Analytics.Client.Entities
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public DateTimeOffset Created { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Role { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
