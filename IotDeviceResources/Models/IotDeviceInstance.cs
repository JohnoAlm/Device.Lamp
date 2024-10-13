using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices;

namespace IotDeviceResources.Models;

public class IotDeviceInstance
{
    public string? ConnectionString { get; set; }
    public Device? Device { get; set; }
    public Twin? Twin { get; set; }
}
