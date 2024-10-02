﻿using Device.Lamp.MVVM.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Text;

namespace Device.Lamp.Handlers;

public class DeviceClientHandler
{
    public IotDevice IotDevice { get; private set; } = new();
    private DeviceClient? _deviceClient;

    public DeviceClientHandler(string deviceId, string deviceType)
    {
        IotDevice!.DeviceId = deviceId;
        IotDevice.DeviceType = deviceType;
        IotDevice.ConnectionString = "HostName=OliverA-IoTHub.azure-devices.net;DeviceId=2bea2269-c1da-4d95-87c3-89af0592f5c3;SharedAccessKey=Zed1Ti0EoIKyGz8quLC2surDhEfYHqL3SnYqURJjkD0=";
    }

    public  ResponseResult<string> Initialize()
    {
        var responseResult = new ResponseResult<string>();

        try
        {
            _deviceClient = DeviceClient.CreateFromConnectionString(IotDevice.ConnectionString);

            if (_deviceClient != null)
            {
                _deviceClient.SetConnectionStatusChangesHandler(ConnectionStatusChangeHandler);

                Task.WhenAll(
                    _deviceClient.SetMethodDefaultHandlerAsync(DirectMethodDefaultCallback, null),
                    UpdateDeviceTwinPropertiesAsync()
                );

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

    public ResponseResult<string> Disconnect()
    {
        var response = new ResponseResult<string>();
       
        try
        {
            IotDevice.DeviceState = false;
            Task.Run(UpdateDeviceTwinPropertiesAsync);
            UpdateDeviceTwinConnectionStateAsync(false).Wait();

            response.Succeeded = true;
            response.Message = "Device disconnected successfully.";
        }
        catch (Exception ex)
        {
            response.Succeeded = false;
            response.Message = ex.Message;
        }

        return response;
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
        IotDevice.DeviceState = true;
        var result = await UpdateDeviceTwinPropertiesAsync();
        if (result.Succeeded)
            return CreateMethodResponse("DeviceState changed to on", 200);
        else
            return CreateMethodResponse($"{result.Message}", 400);
    }

    public async Task<MethodResponse> TurnOffAsync()
    {
        IotDevice.DeviceState = false;
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
                ["connectionState"] = true,
                ["deviceType"] = IotDevice.DeviceType,
                ["deviceState"] = IotDevice.DeviceState

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


    public async Task<ResponseResult<string>> UpdateDeviceTwinConnectionStateAsync(bool connectionState)
    {
        var responseResult = new ResponseResult<string>();

        try
        {
            var reportedProperties = new TwinCollection
            {
                ["connectionState"] = connectionState,
            };

            if (_deviceClient != null)
            {
                await _deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
                responseResult.Succeeded = true;
                responseResult.Message = $"Device ConnectionState updated to {connectionState}.";
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


    public void ConnectionStatusChangeHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
    {
        if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Disabled)
            Task.Run(() => UpdateDeviceTwinConnectionStateAsync(false));

        else if (status == ConnectionStatus.Connected)
            Task.Run(() => UpdateDeviceTwinConnectionStateAsync(true));
       
    }
}
