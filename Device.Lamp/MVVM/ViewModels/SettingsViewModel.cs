using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IotDeviceResources.Data;
using IotDeviceResources.Factories;
using IotDeviceResources.Handlers;
using IotDeviceResources.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Device.Lamp.MVVM.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDatabaseContext _context;
    private readonly DeviceRegistrationHandler _deviceRegistrationHandler;
    private readonly DeviceClientHandler _deviceClientHandler;

    [ObservableProperty]
    private string _pageTitle = "Settings";

    [ObservableProperty]
    private string _deviceId;

    [ObservableProperty]
    private string _deviceName;

    [ObservableProperty]
    private string _deviceType;

    [ObservableProperty]
    private string _errorText;
    
    [ObservableProperty]
    private bool _isConfigured = false;

    [ObservableProperty]
    private IotDeviceSettings? _settings;


    public SettingsViewModel(IServiceProvider serviceProvider, IDatabaseContext context, DeviceRegistrationHandler deviceRegistrationHandler, DeviceClientHandler deviceClientHandler)
    {
        _serviceProvider = serviceProvider;
        _context = context;
        _deviceRegistrationHandler = deviceRegistrationHandler;
        _deviceClientHandler = deviceClientHandler;
        GetDeviceSettingsAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    public async Task ConfigureDevice()
    {
        if(string.IsNullOrWhiteSpace(DeviceId) || string.IsNullOrWhiteSpace(DeviceName) || string.IsNullOrWhiteSpace(DeviceType))
        {
            ErrorText = "All fields must be filled in before submitting!";
            return;
        }

        var registrationResponse = await _deviceRegistrationHandler.RegisterDeviceAsync(DeviceId, DeviceName, DeviceType);

        if(registrationResponse == null)
        {
            ErrorText = "A problem occurred while registering the device.";
            return;
        }

        if(string.IsNullOrEmpty(registrationResponse.DeviceId) || string.IsNullOrEmpty(registrationResponse.DeviceName) || string.IsNullOrWhiteSpace(registrationResponse.DeviceType) || string.IsNullOrEmpty(registrationResponse.ConnectionString))
        {
            ErrorText = "A problem occurred while retrieving the device properties.";
            return;
        }

        await _context.SaveSettingsAsync(IotDeviceSettingsFactory.Create(registrationResponse.DeviceId, registrationResponse.DeviceName, registrationResponse.DeviceType, registrationResponse.ConnectionString));

        var initializationResponse = _deviceClientHandler.Initialize(registrationResponse.DeviceId, registrationResponse.DeviceName, registrationResponse.DeviceType, registrationResponse.ConnectionString);

        if (!initializationResponse.Succeeded)
        {
            ErrorText = "An error occurred while initializing the device.";
            return;
        }

        await GetDeviceSettingsAsync();
        ClearForm();
    }

    public void ClearForm()
    {
        DeviceId = string.Empty;
        DeviceName = string.Empty;
        DeviceType = string.Empty;
        ErrorText = string.Empty;
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
