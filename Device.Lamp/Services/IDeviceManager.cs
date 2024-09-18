using Device.Lamp.MVVM.Models;

namespace Device.Lamp.Services;

public interface IDeviceManager
{
    Task DisconnectAsync(CancellationToken ct);
    Task<ResponseResult<string>> SendDataAsync(string content, CancellationToken ct);
}