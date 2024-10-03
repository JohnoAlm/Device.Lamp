using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Lamp.MVVM.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string _pageTitle = "Home";

    [ObservableProperty]
    private bool _isLampOn;

    public HomeViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _isLampOn = false;
    }

    public void OnDeviceStateChanged(bool deviceState)
    {
        if (!deviceState)
            IsLampOn = false;
        else
            IsLampOn = true;
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        var mainWindowModel = _serviceProvider.GetRequiredService<MainWindowModel>(); 
        mainWindowModel.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
    }
}
