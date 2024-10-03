using IotDeviceResources.Models;

namespace IotDeviceResources.Data;

public class SqliteContext : IDatabaseContext
{
    public Task<ResponseResult<IotDeviceEntity>> GetSettingsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ResponseResult> ResetSettingsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ResponseResult> SaveSettingsAsync(IotDeviceEntity iotDeviceEntity)
    {
        throw new NotImplementedException();
    }
}
