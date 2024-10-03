using IotDeviceResources.Models;

namespace IotDeviceResources.Factories;

public static class IotDeviceEntityFactory
{
    public static IotDeviceEntity Create()
    {
        return new IotDeviceEntity()
        {
            Id = Guid.NewGuid().ToString()
        };
    }

    public static IotDeviceEntity Create(string id)
    {
        return new IotDeviceEntity()
        {
            Id = id
        };
    }

    public static IotDeviceEntity Create(string id, string? type, string? connectionString)
    {
        return new IotDeviceEntity()
        {
            Id = id,
            Type = type,
            ConnectionString = connectionString 
        };
    }
}
