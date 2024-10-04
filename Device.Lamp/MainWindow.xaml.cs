using Device.Lamp.MVVM.ViewModels;
using IotDeviceResources.Data;
using System.Windows;

namespace Device.Lamp;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void TopWindowBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }
}