using ClassLibrary.Entities;
using ClassLibrary.Shared.Interfaces;
using InfraLibrary;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyExampleFunction;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Datadog.Logs;
using System;

[assembly: WebJobsStartup(typeof(Startup))]
namespace MyExampleFunction
{
    public class Startup: IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            ConfigureServices(builder.Services)
                .BuildServiceProvider(true);
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            var databaseConnectionString = Environment.GetEnvironmentVariable("DatabaseConnectionString");

            var datadogApiKey = Environment.GetEnvironmentVariable("DatadogApiKey");

            var serviceName = Environment.GetEnvironmentVariable("ServiceName");

            var datadogConfiguration = new DatadogConfiguration(
                url: Environment.GetEnvironmentVariable("DatadogEndpoint") ?? "intake.logs.datadoghq.com",
                port: 10516,
                useSSL: true,
                useTCP: true);

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadName()
                .Enrich.WithThreadId()
                .WriteTo.Console()
                .WriteTo.DatadogLogs(
                    apiKey: datadogApiKey,
                    host: Environment.MachineName,
                    service: serviceName,
                    configuration: datadogConfiguration,
                    logLevel: LogEventLevel.Information
                ).CreateLogger();

            services
                .AddLogging(builder => builder
                    .AddSerilog(Log.Logger))
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(databaseConnectionString))
                .AddScoped<IRepository<Product>, Repository<Product>>();
            return services;
        }
    }
}
