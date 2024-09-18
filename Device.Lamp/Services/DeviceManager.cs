using Device.Lamp.Models;
using Microsoft.Azure.Devices.Client;
using System.Text;

namespace Device.Lamp.Services;

public class DeviceManager : IDeviceManager
{
    private readonly DeviceClient _client;

    public DeviceManager(string connectionString)
    {
        _client = DeviceClient.CreateFromConnectionString(connectionString);
    }

    public async Task DisconnectAsync(CancellationToken ct)
    {
        await _client.CloseAsync(ct);
    }

    public async Task<ResponseResult<string>> SendDataAsync(string content, CancellationToken ct)
    {
        try
        {
            using var message = new Message(Encoding.UTF8.GetBytes(content))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8"
            };

            await _client.SendEventAsync(message);
            return new ResponseResult<string> { Succeeded = true };
        }
        catch (OperationCanceledException)
        {
            return new ResponseResult<string> { Succeeded = false, ErrorMessage = "Operation was cancelled." };
        }
        catch (Exception ex)
        {
            return new ResponseResult<string> { Succeeded = false, ErrorMessage = ex.Message };
        }
    }
}
