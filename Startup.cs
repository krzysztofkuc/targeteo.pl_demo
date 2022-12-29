using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Web.Http;
using targeteo.pl.BackgroundJobs;
using targeteo.pl.Handlers;
using targeteo.pl.Helpers;
using targeteo.pl.Model;

namespace targeteo.pl
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);
            services.AddResponseCompression();


            #region authentication and authorization
            //Authentication
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins( new string[] { "https://targeteo.pl", "https://www.targeteo.pl","https://targeteo.pl", "http://localhost:56796" } ).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                    // builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                        .Build());
            });
            services.AddTransient<IAuthorizationHandler, UserAuthorizationHandler>();
            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationSchemeOptions, UserAuthenticationHandler>(Consts.DefaultAuthenticationScheme,
                    null);

            //Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Consts.ClaimTypeRoles, policy =>
                    policy.Requirements.Add(new UserAuthorizationRequirement()));
            });

            #endregion authentication and authorization

            services.AddAutoMapper(typeof(Startup));
            //services.AddAutoMapper(typeof(Startup));
            //services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddControllers().AddNewtonsoftJson(
              options => {
                  options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                  options.SerializerSettings.ContractResolver = new DefaultContractResolver();
              });

            #region Hangfire
            services.AddHangfire(configuration => configuration
             .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
             .UseSimpleAssemblyNameTypeSerializer()
             .UseRecommendedSerializerSettings()
             .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
             {
                 CommandBatchMaxTimeout = TimeSpan.FromMinutes(2),
                 SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                 QueuePollInterval = TimeSpan.Zero,
                 UseRecommendedIsolationLevel = true,
                 DisableGlobalLocks = true
             }));
            services.AddHangfireServer();

            services.AddEntityFrameworkSqlServer().
            AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=targeteo.pl;Integrated Security=True");
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Error");

                app.UseDeveloperExceptionPage();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "User",
                    pattern: "api/{controller}/{action=Index}/kategoria/{parentCategory}/{childCategory}",
                    defaults: new { controller = "Home", action = "Index", parentCategory = RouteParameter.Optional, childCategory = RouteParameter.Optional }
                );
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.Options.StartupTimeout = new TimeSpan(0, 5, 80);

                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            backgroundJobs.Schedule<CheckOrderShouldBeCanceledJob>(x => x.Init(), TimeSpan.FromMinutes(4));
            backgroundJobs.Schedule<CkeckIfOrderReceiptShouldBeConfirmed>(x => x.Init(), TimeSpan.FromMinutes(30));
            backgroundJobs.Schedule<SendAskForConfirmReceipt>(x => x.Init(), TimeSpan.FromMinutes(30));
        }
    }
}
