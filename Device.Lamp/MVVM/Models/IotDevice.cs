namespace Device.Lamp.MVVM.Models;

public class IotDevice
{
    public string DeviceId { get; set; } = null!;
    public string? DeviceType { get; set; }
    public bool DeviceState { get; set; }

    public string ConnectionString { get; set; } = null!;
    public bool ConnectionState { get; set; }
}
