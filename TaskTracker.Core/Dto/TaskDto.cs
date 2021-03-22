using System;

using TaskTracker.Core.Entities;

namespace TaskTracker.Core.Dto
{
    public class TaskDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }

        public int Price { get; set; }

        public Guid? PopugId { get; set; }
    }
}
