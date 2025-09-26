using Pinger_2.ViewModel;
using System.Windows;

namespace Pinger_2
{
    public partial class DisplayWindow : Window
    {
        private DisplayWindowViewModel _viewModel;
        public DisplayWindow(DisplayWindowViewModel vm)
        {
            _viewModel = vm;
            InitializeComponent();
        }
    }
}