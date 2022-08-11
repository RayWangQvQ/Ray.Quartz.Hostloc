using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Ray.Quartz.Hostloc;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting console host.");

            await Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                {
                    IList<IConfigurationSource> list = configurationBuilder.Sources;
                    list.ReplaceWhile(
                        configurationSource=> configurationSource is EnvironmentVariablesConfigurationSource,
                        new EnvironmentVariablesConfigurationSource()
                        {
                            Prefix = "Ray_Hostloc_"
                        }
                        );
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<HostlocHostedService>();
                })
                .UseSerilog()
                .RunConsoleAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
