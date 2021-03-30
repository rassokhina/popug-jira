using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

using MassTransit;
using Accounting.Core.Data;
using Microsoft.AspNetCore.Authentication;
using Shared;
using Accounting.Client.Consumers;
using Accounting.Core.Services;

namespace Accounting.Client
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

            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<TaskCreatedConsumer>();
            services.AddScoped<UserCreatedConsumer>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<TaskCreatedConsumer>();
                x.AddConsumer<UserCreatedConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ReceiveEndpoint(Constants.TaskQueueAccounting, e =>
                    {
                        e.ConfigureConsumer<TaskCreatedConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(Constants.UserQueueAccounting, e =>
                    {
                        e.ConfigureConsumer<UserCreatedConsumer>(context);
                    });
                    // TODO:
                    // ���������� ������� BalanceReset � ���������� ������������ ����������� � ������� �������
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
                  options.ClientId = "Accounting.Client";
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
                    pattern: "{controller=Task}/{action=Index}/{id?}");
            });

            DefaultContext.ToLatestVersion(serviceProvider);
        }
    }
}