using System.Diagnostics;
using Controller.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var host = Host
        .CreateDefaultBuilder(args)
        .ConfigureServices((builder, services) =>
        {
            services.Configure<DevicePortConfiguration>(builder.Configuration.GetSection("GsmDevice:Interface"));
            services.Configure<DeviceAtCommands>(builder.Configuration.GetSection("GsmDevice:Commands"));
            services.AddSingleton<SerialPortFactory>();
            services.AddScoped<GsmDevice>();
            services.AddHostedService<Controller.Controller>();
        })
        .Build();
    
#if DEBUG
    // uncomment for remote debuging
    for (; ; )
    {
        Console.WriteLine("waiting for debugger attach");
        if (Debugger.IsAttached) break;
        Task.Delay(3000).Wait();
    }
#endif

    await host.RunAsync();
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
