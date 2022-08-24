using System.IO.Ports;
using EnsureThat;
using Microsoft.Extensions.Options;

namespace Controller.Infrastructure;

public class SerialPortFactory
{
    private readonly ILogger<SerialPortFactory> _logger;
    private readonly DevicePortConfiguration _configuration;
    private static SerialPort? _instance;

    public SerialPortFactory(ILogger<SerialPortFactory> logger, IOptions<DevicePortConfiguration> configuration)
    {
        _logger = logger;
        _configuration = configuration.Value;
        _instance = GetOpenPort();
    }

    public SerialPort GetOpenPort()
    {
        if (_instance is not null)
        {
            if (!_instance.IsOpen)
                _instance.Open();
            return _instance;
        }
            

        Ensure.That(_configuration.PortName).IsNotNullOrEmpty();
    
        if (SerialPort.GetPortNames().All(name => name != _configuration.PortName))
            throw new IOException("Requested port is not available");
        
        _logger.LogInformation("Serial port configured on {PortName}", _configuration.PortName);

        var port = new SerialPort
        {
            PortName = _configuration.PortName!,
            BaudRate = _configuration.BaudRate,
            Handshake = _configuration.Handshake,
            NewLine = _configuration.NewLine
        };
        
        port.Open();
        return port;
    }
}