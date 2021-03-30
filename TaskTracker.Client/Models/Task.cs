using System;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Client.Models
{
    public class Task
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        public string UserId { get; set; }
    }
}
