using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

using Analytics.Client.Consumers;
using Analytics.Core.Data;
using Analytics.Core.Queries;

using MassTransit;
using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Shared;

namespace Analytics.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DefaultContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddCors();

            services.AddMediatR(typeof(GetEarnedByManagementQuery).GetTypeInfo().Assembly);

            services.AddScoped<UserCreatedConsumer>();

            services.AddScoped<TaskCreatedConsumer>();
            services.AddScoped<TaskUpdatedConsumer>();
            services.AddScoped<TaskFinishedConsumer>();
            services.AddScoped<TaskAssignedConsumer>();

            services.AddScoped<BalanceChangedConsumer>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<TaskCreatedConsumer>();
                x.AddConsumer<UserCreatedConsumer>();
                x.AddConsumer<TaskUpdatedConsumer>();
                x.AddConsumer<TaskFinishedConsumer>();
                x.AddConsumer<TaskAssignedConsumer>();
                x.AddConsumer<BalanceChangedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ReceiveEndpoint(Constants.TaskQueueAnalytics, e =>
                    {
                        e.ConfigureConsumer<TaskCreatedConsumer>(context);
                        e.ConfigureConsumer<TaskUpdatedConsumer>(context);
                        e.ConfigureConsumer<TaskFinishedConsumer>(context);
                        e.ConfigureConsumer<TaskAssignedConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(Constants.UserQueueAnalytics, e =>
                    {
                        e.ConfigureConsumer<UserCreatedConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(Constants.BalanceQueueAccounting, e =>
                    {
                        e.ConfigureConsumer<BalanceChangedConsumer>(context);
                    });
                });
            });
            services.AddMassTransitHostedService();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
              .AddCookie("Cookies")
              .AddOpenIdConnect("oidc", options =>
              {
                  options.Authority = "https://localhost:5001";
                  options.SignInScheme = "Cookies";
                  options.ClientId = "Analytics.Client";
                  options.ClientSecret = "secret";
                  options.ResponseType = "code id_token";
                  options.SaveTokens = true;
                  options.GetClaimsFromUserInfoEndpoint = true;

                  options.Scope.Add("roles");
                  options.ClaimActions.MapUniqueJsonKey("role", "role");
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      RoleClaimType = "role"
                  };
              });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Analytics}/{action=Index}/{id?}");
            });

            DefaultContext.ToLatestVersion(serviceProvider);
        }
    }
}
