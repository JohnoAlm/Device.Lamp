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

                services.AddSingleton(new DeviceClientHandler("2bea2269-c1da-4d95-87c3-89af0592f5c3", "lamp"));

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
        using var cts = new CancellationTokenSource();

        try
        {
            await _host!.StartAsync(cts.Token);

            var mainWindow = _host!.Services.GetRequiredService<MainWindow>();
            var deviceClientHandler = _host!.Services.GetRequiredService<DeviceClientHandler>();
            var homeViewModel = _host!.Services.GetRequiredService<HomeViewModel>();
            
            mainWindow.Show();

            var result = deviceClientHandler.Initialize();
            Debug.WriteLine(result.Message);

            deviceClientHandler.IotDevice.DeviceStateChanged += homeViewModel.OnDeviceStateChanged;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using var cts = new CancellationTokenSource();
        
        try
        {
            var deviceClientHandler = _host!.Services.GetRequiredService<DeviceClientHandler>();

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


