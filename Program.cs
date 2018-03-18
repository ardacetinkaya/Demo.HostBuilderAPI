using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SomeKindOfProcessor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
            .ConfigureHostConfiguration(config=>
            {
                config.AddEnvironmentVariables();
            })
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: true);
                config.Build();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ICustomProcessorData>(serviceProvider =>
                {
                    
                    return new CustomProcessorData("Dummy Processor");
                });
                services.AddScoped<IHostedService, CustomProcessor>();
            });

            await hostBuilder.RunConsoleAsync();

        }
    }
}
