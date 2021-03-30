using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Analytics.Client.Entities;
using Analytics.Client.Data.Configuration;

namespace Analytics.Client.Data
{
    public class DefaultContext : DbContext
    {
        public DefaultContext(DbContextOptions<DefaultContext> options)
            : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TaskConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());

            base.OnModelCreating(builder);
        }

        public static void ToLatestVersion(IServiceProvider serviceProvider)
        {
            using var context = new DefaultContext(serviceProvider.GetService<DbContextOptions<DefaultContext>>());
            context.Database.Migrate();
        }
    }
}
