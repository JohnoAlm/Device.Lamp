using IotDeviceResources.Models;
using Microsoft.Azure.Devices;
using System.Diagnostics;

namespace IotDeviceResources.Managers;

public class IotDeviceRegistrationManager
{
    private readonly string? _connectionString;
    private readonly RegistryManager _registryManager;
    private readonly ServiceClient _serviceClient;

    public IotDeviceRegistrationManager(string? connectionString)
    {
        _connectionString = connectionString;
        _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
        _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
    }

    public bool Disconnect()
    {
        try
        {
            _registryManager!.Dispose();
            _serviceClient!.Dispose();

            if (_registryManager == null && _serviceClient == null)
                return true;

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<IotDeviceInstance> RegisterDeviceAsync(string deviceId, string deviceName, string deviceType)
    {
        if (string.IsNullOrEmpty(deviceId))
            return null!;

        var iotDeviceInstance = new IotDeviceInstance
        {
            Device = await _registryManager!.GetDeviceAsync(deviceId) ?? await _registryManager.AddDeviceAsync(new Device(deviceId))
        };

        await UpdateDesiredPropertyAsync(iotDeviceInstance.Device, nameof(deviceName), deviceName);
        await UpdateDesiredPropertyAsync(iotDeviceInstance.Device, nameof(deviceType), deviceType);

        iotDeviceInstance.ConnectionString = GetDeviceConnectionString(iotDeviceInstance.Device);
        iotDeviceInstance.Twin = await _registryManager.GetTwinAsync(iotDeviceInstance.Device.Id);

        return iotDeviceInstance;
    }

    public string GetDeviceConnectionString(Device device)
    {
        var iotDeviceConnectionString = $"{_connectionString!.Split(";")[0]};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
        return iotDeviceConnectionString ?? null!;
    }

    public async Task<bool> UpdateDesiredPropertyAsync(Device device, string key, string value)
    {
        try
        {
            var twin = await _registryManager!.GetTwinAsync(device.Id);
            twin.Properties.Desired[key] = value;

            await _registryManager.UpdateTwinAsync(device.Id, twin, twin.ETag);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }
}
