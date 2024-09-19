using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Lamp.MVVM.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    public HomeViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [ObservableProperty]
    private string _pageTitle = "Home";

    [RelayCommand]
    private void NavigateToSettings()
    {
        var mainWindowModel = _serviceProvider.GetRequiredService<MainWindowModel>(); 
        mainWindowModel.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
    }
}
