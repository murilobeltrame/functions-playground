using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MyExampleFunction;
using Serilog;

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
            services
                .AddLogging(builder => builder
                    .AddSerilog(new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .Enrich.WithThreadId()
                        .Enrich.WithThreadName()
                        .WriteTo.Console()
                        //.WriteTo.DatadogLogs()
                        .CreateLogger()
                        ));
            return services;
        }
    }
}
