
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Targets.RedisJson.Example;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets.RedisJson;

// If you install from nuget remove this
LogManager.Setup().SetupExtensions(ext =>
{
    ext.RegisterTarget<RedisJsonTarget>();
});

var logger = LogManager.GetCurrentClassLogger();

// Setup like ASP.NET application
try
{
    var config = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .Build();

    using var servicesProvider = new ServiceCollection()
        .AddTransient<Runner>()
        .AddLogging(loggingBuilder =>
        {
            // configure Logging with NLog
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            loggingBuilder.AddNLog(config);
        }).BuildServiceProvider();

    var runner = servicesProvider.GetRequiredService<Runner>();

    for (int i = 0; i < 2; i++)
    {
        runner.DoAction("Action_" + i);
        runner.DoError("ErrorAction_" + i);
    }

    Console.WriteLine("Press ANY key to exit");
    Console.ReadKey();
}
catch (Exception ex)
{
    // NLog: catch any exception and log it.
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}