using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Services.MessageManager;
using SoMeSimulator.Services;
using SoMeSimulator.Services.SignalR;
using SoMeSimulator.Services.SignalRHubs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SomeSimulator.Data.Models.Configurations;
using SomeSimulator.DTOs;
using SomeSimulator.Interfaces;
using SomeSimulator.Services;
using SomeSimulator.Services.FakeAliasService;

namespace SomeSimulator
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
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            services.AddCors();

            if (environment == EnvironmentName.Development)
            {
                services.AddDbContext<SoMeContext>(options =>
                    options
                        .UseLazyLoadingProxies()
                        .UseMySql(Configuration.GetConnectionString("SoMeContextConnectionString")));
            }
            else if(environment == EnvironmentName.Production)
            {
                services.AddDbContext<SoMeContext>(options =>
                    options
                        .UseLazyLoadingProxies()
                        .UseMySql(Configuration.GetConnectionString("SoMeContextConnectionString")));
            }
            else
            {
                services.AddDbContext<SoMeContext>(options =>
                    options
                        .UseLazyLoadingProxies()
                        .UseMySql(Configuration.GetConnectionString("SoMeContextConnectionString")));
            }


            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); 
                });


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                });

            services.AddSignalR()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
            //    services.Configure<CookiePolicyOptions>(options => {
            //        options.CheckConsentNeeded = context => true;
            //        options.MinimumSameSitePolicy = SameSiteMode.None;
            //    });

            services.AddAutoMapper(typeof(Startup));
            
            services.AddHttpContextAccessor();
            services.AddHostedService<TimedHostedService>();
            services.AddTransient<ISendMessage, SendMessage>();
            services.AddTransient<IEntityFactory, EntityFactory>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRemove, Remove>();
            services.AddTransient<ICrudInterface<ScenarioDto>, ScenarioService>();
            services.AddTransient<ICrudInterface<ScenarioEventDto>, ScenarioEventService>();
            services.AddTransient<ICrudInterface<PostDto>, PostService>();
            services.AddTransient<ICrudInterface<CommentDto>, CommentService>();
            services.AddTransient<IStressLevelCalculator, StressLevelCalculator>();
            services.AddTransient<IFakeAlias, FakeAlias>();
            services.Configure<StressLevel>(Configuration.GetSection("StressLevel"));
            services.Configure<CommentsSettings>(Configuration.GetSection("CommentsSettings"));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                // run migration on startup
                MigrateDatabase(app);
            }
            else
            {
                //Uncomment to run migrations in prod.
                // MigrateDatabase(app);

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseCors(options => {
                options
                    .WithOrigins("http://localhost:8080","http://localhost:8081","http://localhost:8082")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
            //app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions() {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            });
            
            app.Use(async (context, next) =>
            {
                //Signal R
                if(context.Request.Path.Value.ToLower().Contains("sr")){
                    await next();
                    return;
                }
                    

                //Api
                if (context.Request.Path.Value.ToLower().Contains("api"))
                {
                    await next();
                    return;
                }
                    
                if(!Path.HasExtension(context.Request.Path.Value) && context.Request.HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest" ) {
                    await context.Response.WriteAsync(System.IO.File.ReadAllText("wwwroot/index.html"));
                    return;
                }
                
                await next();
            });

            app.UseMvc();
            app.UseWebSockets();
            app.UseSignalR(route => { route.MapHub<MessageHub>("/sr"); });
        }

        private static void MigrateDatabase(IApplicationBuilder app)
        {

            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<SoMeContext>())
                {
                    //if (!context.Database.EnsureCreated())
                        context.Database.Migrate();
                }
            }
        }
    }
}
