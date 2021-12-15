using ClassLibrary.Entities;
using ClassLibrary.Shared.Interfaces;
using InfraLibrary;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyExampleFunction;
using Serilog;
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

            services
                .AddLogging(builder => builder
                    .AddSerilog(new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .Enrich.WithThreadId()
                        .Enrich.WithThreadName()
                        .WriteTo.Console()
                        //.WriteTo.DatadogLogs()
                        .CreateLogger()
                        ))
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(databaseConnectionString))
                .AddScoped<IRepository<Product>, Repository<Product>>();
            return services;
        }
    }
}
