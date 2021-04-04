using System;
using Accounting.Core.Data.Configuration;
using Accounting.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Core.Data
{
    public class DefaultContext : DbContext
    {
        public DefaultContext(DbContextOptions<DefaultContext> options)
            : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<WalletAudit> WalletAudits { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TaskConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new WalletConfiguration());
            builder.ApplyConfiguration(new WalletAuditConfiguration());
            base.OnModelCreating(builder);
        }

        public static void ToLatestVersion(IServiceProvider serviceProvider)
        {
            using var context = new DefaultContext(serviceProvider.GetService<DbContextOptions<DefaultContext>>());
            context.Database.SetCommandTimeout(600);
            context.Database.Migrate();
        }
    }
}
