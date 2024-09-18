using Device.Lamp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Device.Lamp;

public partial class App : Application
{
    private static IHost? _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
        {

            //services.AddSingleton<IDeviceManager>(new DeviceManager(""));

            services.AddSingleton<MainWindow>();

        }).Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        var mainWindow = _host!.Services.GetRequiredService<MainWindow>();   
        mainWindow.Show();

        using var cts = new CancellationTokenSource();
        try
        {
            await _host!.RunAsync(cts.Token);
        }
        catch { }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        //var deviceManager = _host!.Services.GetRequiredService<IDeviceManager>();

        using var cts = new CancellationTokenSource();
        
        try
        {
            //await deviceManager.DisconnectAsync(cts.Token);
            await _host!.StopAsync(cts.Token);
        }
        catch { }

        base.OnExit(e);
    }
}
