using Pinger_2.ViewModel;
using System.Windows;

namespace Pinger_2
{
    public partial class DisplayWindow : Window
    {
        
        public DisplayWindow(DisplayWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}