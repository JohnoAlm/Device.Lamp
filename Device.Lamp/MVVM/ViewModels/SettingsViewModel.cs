using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Lamp.MVVM.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    public SettingsViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [ObservableProperty]
    private string _pageTitle = "Settings";

    [RelayCommand]
    private void NavigateToHome()
    {
        var mainWindowModel = _serviceProvider.GetRequiredService<MainWindowModel>();
        mainWindowModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomeViewModel>();
    }
}
