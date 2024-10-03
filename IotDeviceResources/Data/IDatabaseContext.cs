using IotDeviceResources.Models;

namespace IotDeviceResources.Data;

public interface IDatabaseContext
{
    Task<ResponseResult<IotDeviceEntity>> GetSettingsAsync();
    Task<ResponseResult> ResetSettingsAsync();
    Task<ResponseResult> SaveSettingsAsync(IotDeviceEntity iotDeviceEntity);
}
