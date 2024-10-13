using IotDeviceResources.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace TestProject
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ReadKey();  

            var iotDeviceRegistrationRequest = new IotDeviceRegistrationRequest
            {
                DeviceId = "TestId",
                DeviceName = "TestName",
                DeviceType = "TestType"
            };

            using var http = new HttpClient();
            var result = await http.PostAsJsonAsync("https://olivera-iot-fa.azurewebsites.net/api/IotDeviceRegistration?code=j7vtWmoSy6LkwDwnn8w3LT19tPQk7DvrDMJmGp_X8W6dAzFucqKn7g%3D%3D", iotDeviceRegistrationRequest);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<IotDeviceRegistrationResponse>(content);

            DeviceClient client = DeviceClient.CreateFromConnectionString(response!.ConnectionString);

            var twinCollection = new TwinCollection();
            twinCollection["deviceName"] = response.DeviceName;
            twinCollection["deviceType"] = response.DeviceType;

            await client.UpdateReportedPropertiesAsync(twinCollection);

            Console.ReadKey();
        }
    }
}
