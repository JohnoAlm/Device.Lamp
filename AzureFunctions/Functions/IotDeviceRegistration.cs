using IotDeviceResources.Managers;
using IotDeviceResources.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions.Functions
{
    public class IotDeviceRegistration
    {
        private readonly ILogger<IotDeviceRegistration> _logger;
        private readonly IotDeviceRegistrationManager _iotDeviceRegistrationManager;

        public IotDeviceRegistration(ILogger<IotDeviceRegistration> logger, IotDeviceRegistrationManager iotDeviceRegistrationManager)
        {
            _logger = logger;
            _iotDeviceRegistrationManager = iotDeviceRegistrationManager;
        }

        [Function("IotDeviceRegistration")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var iotDeviceRegistrationRequest = JsonConvert.DeserializeObject<IotDeviceRegistrationRequest>(body);

            if (iotDeviceRegistrationRequest == null || string.IsNullOrEmpty(iotDeviceRegistrationRequest.DeviceId) || string.IsNullOrEmpty(iotDeviceRegistrationRequest.DeviceName) || string.IsNullOrEmpty(iotDeviceRegistrationRequest.DeviceType))
                return new BadRequestObjectResult("Invalid request. 'deviceId', 'deviceName' or 'deviceType' is missing.");

            var result = await _iotDeviceRegistrationManager.RegisterDeviceAsync(iotDeviceRegistrationRequest.DeviceId, iotDeviceRegistrationRequest.DeviceName, iotDeviceRegistrationRequest.DeviceType);

            var response = new IotDeviceRegistrationResponse
            {
                DeviceId = result.Device?.Id,
                ConnectionString = result.ConnectionString,
                DeviceName = result.Twin?.Properties.Desired["deviceName"].ToString(),
                DeviceType = result.Twin?.Properties.Desired["deviceType"].ToString()
            };

            return new OkObjectResult(response);
        }
    }
}
