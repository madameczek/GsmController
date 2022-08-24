using System.IO.Ports;
using EnsureThat;
using Microsoft.Extensions.Options;

namespace Controller.Infrastructure;

public class GsmDevice
{
    private readonly ILogger<GsmDevice> _logger;
    private readonly SerialPortFactory _portFactory;
    private readonly DeviceAtCommands _configuration;
    public GsmDevice(ILogger<GsmDevice> logger, IOptions<DeviceAtCommands> configuration, SerialPortFactory portFactory)
    {
        _logger = logger;
        _portFactory = portFactory;
        _configuration = configuration.Value;
    } 

    public async Task Initialize()
    {
        try
        {
            Ensure.That(_configuration.Init).IsNotNull();
            
            var port = _portFactory.GetOpenPort();
            
            foreach (var command in _configuration.Init!)
            {
                port.WriteLine(command);
                _logger.LogInformation("Initialise modem: {Command}", command);
                await Task.Delay(200);
            }
            port.DiscardInBuffer();
            port.Close();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Modem initialization error");
        }
    }

    public void ReadDevice(SerialDataReceivedEventHandler handler, CancellationToken cancellationToken)
    {
        try
        {
            var port = _portFactory.GetOpenPort();
            port.DataReceived += handler;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}