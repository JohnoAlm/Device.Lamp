using Device.Lamp.MVVM.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Text;

namespace Device.Lamp.Handlers;

public class DeviceClientHandler
{
    private readonly IotDevice _iotDevice = new();
    private DeviceClient? _deviceClient;

    public DeviceClientHandler(string deviceId, string deviceType)
    {
        _iotDevice!.DeviceId = deviceId;
        _iotDevice.DeviceType = deviceType;
    }

    public async Task<ResponseResult<string>> InitializeAsync()
    {
        var responseResult = new ResponseResult<string>();

        try
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(_iotDevice.ConnectionString);

            if (_deviceClient != null)
            {
                await _deviceClient.SetMethodDefaultHandlerAsync(DirectMethodDefaultCallback, null);
                await UpdateDeviceTwinPropertiesAsync();

                responseResult.Succeeded = true;
                responseResult.Message = "Device initialized.";
            }
            else
            {
                responseResult.Succeeded = false;
                responseResult.Message = "Device client not found.";
            }
        }
        catch (Exception ex) 
        {
            responseResult.Succeeded = false;
            responseResult.Message = ex.Message;
        }

        return responseResult;
    }

    public async Task<MethodResponse> DirectMethodDefaultCallback(MethodRequest request, object userContext)
    {
        var methodResponse = request.Name.ToLower() switch
        {
            "on" => await TurnOnAsync(),
            "off" => await TurnOffAsync(),
            _ => CreateMethodResponse("No suitable method found", 404)
        };

        return methodResponse;
    }

    public async Task<MethodResponse> TurnOnAsync()
    {
        _iotDevice.DeviceState = true;
        var result = await UpdateDeviceTwinPropertiesAsync();
        if(result.Succeeded)
            return CreateMethodResponse("DeviceState changed to on", 200);
        else
            return CreateMethodResponse($"{result.Message}", 400);
    }

    public async Task<MethodResponse> TurnOffAsync()
    {
        _iotDevice.DeviceState = false;
        var result = await UpdateDeviceTwinPropertiesAsync();
        if (result.Succeeded)
            return CreateMethodResponse("DeviceState changed to off", 200);
        else
            return CreateMethodResponse($"{result.Message}", 400);
    }

    public MethodResponse CreateMethodResponse(string message, int statusCode)
    {
        try
        {
            var json = JsonConvert.SerializeObject(new { Message = message });
            var methodResponse = new MethodResponse(Encoding.UTF8.GetBytes(json), statusCode);
            return methodResponse;
        }
        catch (Exception ex) 
        {
            var json = JsonConvert.SerializeObject(new { Message = ex.Message });
            var methodResponse = new MethodResponse(Encoding.UTF8.GetBytes(json), statusCode);
            return methodResponse;
        }
    }

    public async Task<ResponseResult<string>> UpdateDeviceTwinPropertiesAsync()
    {
        var responseResult = new ResponseResult<string>();

        try
        {
            var reportedProperties = new TwinCollection
            {
                ["connectionState"] = _iotDevice.ConnectionState,
                ["deviceType"] = _iotDevice.DeviceType,
                ["deviceState"] = _iotDevice.DeviceState

            };

            if (_deviceClient != null)
            {
                await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
                responseResult.Succeeded = true;
            }
            else
            {
                responseResult.Succeeded = false;
                responseResult.Message = "Device client not found.";
            }
        }
        catch (Exception ex)
        {
            responseResult.Succeeded = false;
            responseResult.Message = ex.Message;
        }

        return responseResult;
    }

}
