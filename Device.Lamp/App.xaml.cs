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

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _host!.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        var deviceClientHandler = _host!.Services.GetRequiredService<DeviceClientHandler>();
        var homeViewModel = _host!.Services.GetRequiredService<HomeViewModel>();

        using var cts = new CancellationTokenSource();

        try
        {
            var result = deviceClientHandler.Initialize();
            Debug.WriteLine(result.Message);

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


