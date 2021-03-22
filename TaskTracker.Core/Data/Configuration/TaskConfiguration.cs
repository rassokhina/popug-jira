using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TaskTracker.Core.Entities;

namespace TaskTracker.Core.Data.Configuration
{
    internal sealed class TaskConfiguration :  IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {

        }
    }
}
