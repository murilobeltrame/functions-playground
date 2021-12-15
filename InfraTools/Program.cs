using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace InfraTools
{
    class Program
    {
        protected Program() { }

        static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    var defaultConnectionString = context.Configuration["ConnectionStrings:Default"];
                    Console.WriteLine($"ConnString is {defaultConnectionString}");
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(defaultConnectionString));
                });
        }
    }
}
