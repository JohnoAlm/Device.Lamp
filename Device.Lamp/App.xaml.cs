using Device.Lamp.MVVM.ViewModels;
using Device.Lamp.MVVM.Views;
using IotDeviceResources.Data;
using IotDeviceResources.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Windows;

namespace Device.Lamp;

public partial class App : Application
{
    private static IHost? _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
       
            .ConfigureServices(services =>
            {
                services.AddLogging();

                services.AddSingleton<IDatabaseContext>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<SqliteContext>>();
                    var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    return new SqliteContext(logger, () => directoryPath);
                });

                services.AddSingleton<DeviceClientHandler>();
                services.AddTransient<DeviceRegistrationHandler>();

                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowModel>();

                services.AddSingleton<HomeView>();
                services.AddSingleton<HomeViewModel>();

                services.AddSingleton<SettingsView>();
                services.AddSingleton<SettingsViewModel>();

            }).Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _host!.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        var context = _host!.Services.GetRequiredService<IDatabaseContext>();
        var deviceClientHandler = _host!.Services.GetRequiredService<DeviceClientHandler>();
        var homeViewModel = _host!.Services.GetRequiredService<HomeViewModel>();

        using var cts = new CancellationTokenSource();

        try
        {
            var response = await context.GetSettingsAsync();

            if(response.Succeeded && response.Content != null)
            {
                if(string.IsNullOrEmpty(response.Content.Id) || string.IsNullOrEmpty(response.Content.Name) || string.IsNullOrEmpty(response.Content.Type) || string.IsNullOrEmpty(response.Content.ConnectionString))
                    Debug.WriteLine("Could not retrieve all device properties.");        
                else
                {
                    var result = deviceClientHandler.Initialize(response.Content.Id, response.Content.Name, response.Content.Type, response.Content.ConnectionString);
                    Debug.WriteLine(result.Message);
                }
            }
            else
            {
                Debug.WriteLine(response.Message);
            }

            deviceClientHandler.IotDevice.DeviceStateChanged += homeViewModel.OnDeviceStateChanged;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

    }

    protected override async void OnExit(ExitEventArgs e)
    {
        var deviceClientHandler = _host!.Services.GetRequiredService<DeviceClientHandler>();

        using var cts = new CancellationTokenSource();

        try
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                var result = deviceClientHandler.Disconnect();
                Debug.WriteLine(result.Message);
            };

            await _host!.StopAsync(cts.Token);
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        base.OnExit(e);
    }
}


