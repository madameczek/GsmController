using System.IO.Ports;

namespace Controller.Infrastructure;

public class DevicePortConfiguration
{
    public string? PortName { get; set; }
    public int BaudRate { get; set; }
    public Handshake Handshake { get; set; }
    public string NewLine { get; set; }
}