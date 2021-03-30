using Accounting.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Core.Data.Configuration
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(x => x.Tasks)
                  .WithOne(p => p.User)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
