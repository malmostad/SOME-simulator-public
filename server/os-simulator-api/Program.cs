using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace SomeSimulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == EnvironmentName.Development;
            
            var logPath = environment == EnvironmentName.Production ? "log/log.txt": AppDomain.CurrentDomain.BaseDirectory + "log/log-{Date}.txt";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile(logPath)
                .CreateLogger();

            
            try
            {
                Log.Information("Starting web host");
                
                if(environment == EnvironmentName.Production){
                    CreateWebHostBuilder(args)
                        /*.UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseUrls("http://*:5000")*/
                        .Build().Run();
                }
                else {
                    CreateWebHostBuilder(args)
                        .Build().Run();
                }
                
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true")
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
