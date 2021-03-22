using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TaskTracker.Core.Entities;

namespace TaskTracker.Core.Data.Configuration
{
    internal sealed class PopugConfiguration : IEntityTypeConfiguration<Popug>
    {
        public void Configure(EntityTypeBuilder<Popug> builder)
        {
            builder.HasMany(x => x.Tasks)
                  .WithOne(p => p.Popug)
                  .HasForeignKey(p => p.PopugId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
