using IotDeviceResources.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Json;

namespace IotDeviceResources.Handlers;

public class DeviceRegistrationHandler
{
    public async Task<IotDeviceRegistrationResponse> RegisterDeviceAsync(string deviceId, string deviceName, string deviceType)
    {
        var iotDeviceRegistrationRequest = new IotDeviceRegistrationRequest
        {
            DeviceId = deviceId,
            DeviceName = deviceName,
            DeviceType = deviceType
        };

        try
        {
            using var http = new HttpClient();
            var result = await http.PostAsJsonAsync("https://olivera-iot-fa.azurewebsites.net/api/IotDeviceRegistration?code=j7vtWmoSy6LkwDwnn8w3LT19tPQk7DvrDMJmGp_X8W6dAzFucqKn7g%3D%3D", iotDeviceRegistrationRequest);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<IotDeviceRegistrationResponse>(content);

            return response!;
        }
        catch (Exception ex) 
        {
            Debug.Write(ex.Message);
            return null!;
        }
    }
}
