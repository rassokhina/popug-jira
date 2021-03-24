using System;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Web.Models
{
    public class Task
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        public int Price { get; set; }

        public Guid? PopugId { get; set; }
    }
}
