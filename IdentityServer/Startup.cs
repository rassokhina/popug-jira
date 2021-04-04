using System;
using System.Linq;

using System.Reflection;
using System.Threading.Tasks;

using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Shared.Events;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(builder =>
                builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sqlOptions => 
                    sqlOptions.MigrationsAssembly(migrationAssembly)));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(opt =>
                {
                    opt.ConfigureDbContext = c => c.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                        sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                        sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddAspNetIdentity<IdentityUser>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq();
            });

            services.AddMassTransitHostedService();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            InitializeDbTestData(app).Wait();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private static async Task InitializeDbTestData(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            await serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();
            await serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
            await serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            var publishEndpoint = serviceScope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients())
                {
                    await context.Clients.AddAsync(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetIdentityResources())
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var scope in Config.GetApiScopes())
                {
                    await context.ApiScopes.AddAsync(scope.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.GetApiResources())
                {
                    await context.ApiResources.AddAsync(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            if (!userManager.Users.Any())
            {
                foreach (var testUser in Config.GetUsers())
                {
                    var identityUser = new IdentityUser(testUser.Username)
                    {
                        Id = testUser.SubjectId
                    };

                    await userManager.CreateAsync(identityUser, testUser.Password);
                    userManager.AddClaimsAsync(identityUser, testUser.Claims.ToList()).Wait();

                    // stream users to other services 
                    await publishEndpoint.Publish(new UserCreatedEvent
                    {
                        EventId = Guid.NewGuid(),
                        EventName = nameof(UserCreatedEvent),
                        EventTime = DateTimeOffset.UtcNow,
                        EventProducer = "IdentityServer",
                        PublicId = identityUser.Id,
                        Username = identityUser.UserName,
                        Role = testUser.Claims.First().Value
                    });
                }
            }
        }
    }
}
