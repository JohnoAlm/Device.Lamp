namespace IotDeviceResources.Models;

public class IotDeviceRegistrationRequest
{
    public string DeviceId { get; set; } = null!;
    public string DeviceName { get; set; } = null!; 
    public string DeviceType { get; set; } = null!;
}
