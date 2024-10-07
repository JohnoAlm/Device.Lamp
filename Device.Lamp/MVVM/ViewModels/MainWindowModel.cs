using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Device.Lamp.MVVM.ViewModels;

public partial class MainWindowModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableObject _currentViewModel;

    public MainWindowModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CurrentViewModel = _serviceProvider.GetRequiredService<HomeViewModel>();
    }

    [RelayCommand]
    private void CloseApp()
    {
        Application.Current.Shutdown();
    }
}
