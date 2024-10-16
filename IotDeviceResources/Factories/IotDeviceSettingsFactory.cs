﻿using IotDeviceResources.Models;

namespace IotDeviceResources.Factories;

public static class IotDeviceSettingsFactory
{
    public static IotDeviceSettings Create()
    {
        return new IotDeviceSettings()
        {
            Id = Guid.NewGuid().ToString()
        };
    }

    public static IotDeviceSettings Create(string id)
    {
        return new IotDeviceSettings()
        {
            Id = id
        };
    }

    public static IotDeviceSettings Create(string id, string? name, string? type, string? connectionString)
    {
        return new IotDeviceSettings()
        {
            Id = id,
            Name = name,
            Type = type,
            ConnectionString = connectionString 
        };
    }
}
