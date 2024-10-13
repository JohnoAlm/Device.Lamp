using IotDeviceResources.Managers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton(new IotDeviceRegistrationManager(Environment.GetEnvironmentVariable("IotDeviceRegistrationManagerCS")));
    })
    .Build();

host.Run();
