using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media;

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

    [RelayCommand]
    private void ToggleLamp()
    {
        IsLampOn = !IsLampOn;
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        var mainWindowModel = _serviceProvider.GetRequiredService<MainWindowModel>(); 
        mainWindowModel.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
    }
}
