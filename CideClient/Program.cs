using System.Threading.Tasks;
using EliteAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Somfic.Logging.Console;
using Somfic.Logging.Console.Themes;
using System.Windows;
using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace CideClient
{
    // Creates a new IHost for dependency injection
    // Creates a new instance of our Core class and starts it
    internal class Program
    {
        [STAThread]
        private static async Task Main(string[] args)
        {
            AppCenter.Start("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
                   typeof(Analytics), typeof(Crashes));
            await Analytics.SetEnabledAsync(true);
            AppCenter.LogLevel = Microsoft.AppCenter.LogLevel.Verbose;

            // Build the host for dependency injection
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging((context, logger) =>
                {
                    logger.ClearProviders();
                    logger.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    logger.AddPrettyConsole(ConsoleThemes.Code);
                })
                .ConfigureServices((context, service) =>
                {
                    // Add EliteAPI's services to the depdency injection system
                    service.AddEliteAPI();
                })
                .Build();

            // Create an instance of our Core class
            var core = ActivatorUtilities.CreateInstance<Core>(host.Services);

            // Execute the Run method inside our Core class
            await core.Run();
            // Run forever
            await Task.Delay(-1);
        }
    }
}