namespace Controller.Infrastructure;

public class DeviceAtCommands
{
    public string? AccountBalanceQuery { get; set; }
    public string? FlightModeOn { get; set; }
    public string? FlightModeOff { get; set; }
    public IReadOnlyList<string>? Init { get; set; }
}
