using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Device.Lamp.MVVM.ViewModels;

public partial class MainWindowModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    public MainWindowModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CurrentViewModel = _serviceProvider.GetRequiredService<HomeViewModel>();
    }

    [ObservableProperty]
    private ObservableObject _currentViewModel;

    [RelayCommand]
    private void CloseApp()
    {
        Environment.Exit(0);
    }
}
