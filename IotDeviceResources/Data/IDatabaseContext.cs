using IotDeviceResources.Models;

namespace IotDeviceResources.Data;

public interface IDatabaseContext
{
    Task<ResponseResult<IotDeviceSettings>> GetSettingsAsync();
    Task<ResponseResult> ResetSettingsAsync();
    Task<ResponseResult> SaveSettingsAsync(IotDeviceSettings iotDeviceEntity);
}
