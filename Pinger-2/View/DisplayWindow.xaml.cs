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
            vm.StartPingingAll();

        }

        private void DragField_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeFormButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseFormButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
            base.OnStateChanged(e);
        }
    }
}