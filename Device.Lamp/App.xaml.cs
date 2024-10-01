using Device.Lamp.Handlers;
using Device.Lamp.MVVM.ViewModels;
using Device.Lamp.MVVM.Views;
using Device.Lamp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Windows;

namespace Device.Lamp;

public partial class App : Application
{
    private static IHost? _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
        {
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
        catch { }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using var cts = new CancellationTokenSource();
        
        try
        {
            await _host!.StopAsync(cts.Token);
        }
        catch { }

        base.OnExit(e);
    }
}
