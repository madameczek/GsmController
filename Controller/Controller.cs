using System.IO.Ports;
using Controller.Infrastructure;
using Microsoft.Extensions.Options;
using static System.Threading.Tasks.Task;

namespace Controller;

public class Controller : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(200));
    private readonly ILogger<Controller> _logger;
    private readonly IServiceProvider _provider;
    private readonly DevicePortConfiguration _configuration;
    private IServiceScope? _scope;

    public Controller(ILogger<Controller> logger, IOptions<DevicePortConfiguration> configuration, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;
        _configuration = configuration.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        using (_scope = _provider.CreateScope())
        {
            var service = _scope.ServiceProvider.GetRequiredService<GsmDevice>();
            await Delay(TimeSpan.FromSeconds(1), cancellationToken);
            await service.Initialize();
        }
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (_scope = _provider.CreateScope())
        {
            var service = _scope.ServiceProvider.GetRequiredService<GsmDevice>();

            /*while (//await _timer.WaitForNextTickAsync(stoppingToken) &&
                !stoppingToken.IsCancellationRequested)*/
            {
                service.ReadDevice(new SerialDataReceivedEventHandler(DataReceivedHandler), stoppingToken);
            }
        }
    }

    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        var port = (SerialPort)sender;
        while (port.BytesToRead > 0)
        {
            var indata = port.ReadTo("\r\n");
            if (!string.IsNullOrWhiteSpace(indata)) _logger.LogInformation(indata);
        }
    }
}