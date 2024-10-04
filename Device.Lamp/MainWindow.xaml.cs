using Device.Lamp.MVVM.ViewModels;
using IotDeviceResources.Data;
using System.Windows;

namespace Device.Lamp;

public partial class MainWindow : Window
{
    private readonly IDatabaseContext _contex;

    public MainWindow(MainWindowModel viewModel, IDatabaseContext contex)
    {
        InitializeComponent();
        DataContext = viewModel;
        _contex = contex;
    }

    private void TopWindowBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }
}