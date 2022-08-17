using System.Reflection;
using ET.Common.Configuration;
using ET.Common.Logging;
using Serilog;
namespace OrderApi;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = LogHelper.CreateInitialLogger();

        try
        {
            Log.Information($"Starting up; Version: {Assembly.GetExecutingAssembly().GetName().Version}");
            await CreateHostBuilder(args).Build().RunAsync();
            Log.Information($"Started up");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Start-up failed");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog(LogHelper.ConfigureLogger)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                var configuration = ConfigurationHelper.GetCommonConfiguration<Program>();
                webBuilder
                    .UseConfiguration(configuration)
                    .UseStartup<Startup>();
            });

}