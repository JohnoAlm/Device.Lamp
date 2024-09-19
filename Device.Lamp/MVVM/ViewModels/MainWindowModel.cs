using CommunityToolkit.Mvvm.ComponentModel;
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
}
