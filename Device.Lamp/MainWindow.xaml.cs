using Device.Lamp.MVVM.ViewModels;
using System.Windows;

namespace Device.Lamp;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;    
    }
}