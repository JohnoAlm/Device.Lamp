using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IotDeviceResources.Data;
using IotDeviceResources.Factories;
using IotDeviceResources.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Lamp.MVVM.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDatabaseContext _context;

    [ObservableProperty]
    private bool _isConfigured = false;

    [ObservableProperty]
    private IotDeviceSettings? _settings;

    [ObservableProperty]
    private string _pageTitle = "Settings";

    public SettingsViewModel(IServiceProvider serviceProvider, IDatabaseContext context)
    {
        _serviceProvider = serviceProvider;
        _context = context;
        GetDeviceSettingsAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    public async Task ConfigureSettings()
    {
        await _context.SaveSettingsAsync(IotDeviceSettingsFactory.Create());
        await GetDeviceSettingsAsync(); 
    }

    [RelayCommand]
    public async Task ResetSettings()
    {
        await _context.ResetSettingsAsync();
        await GetDeviceSettingsAsync();
    }

    public async Task GetDeviceSettingsAsync()
    {
        var response = await _context.GetSettingsAsync();
        Settings = response.Content;
        IsConfigured = Settings != null;
    }

    [RelayCommand]
    private void NavigateToHome()
    {
        var mainWindowModel = _serviceProvider.GetRequiredService<MainWindowModel>();
        mainWindowModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomeViewModel>();
    }
}
