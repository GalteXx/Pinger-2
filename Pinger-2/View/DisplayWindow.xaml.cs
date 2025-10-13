using Pinger_2.ViewModel;
using Pinger_2.WinApi;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Pinger_2
{
    public partial class DisplayWindow : Window
    {
        private readonly NotifyIcon _notifyIcon;
        public DisplayWindow(DisplayWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            string[]? path = Environment.ProcessPath?.Split('\\'); //this is horrible and I hate it
            _notifyIcon = new(this, $"{String.Join('\\', path!.Take(path!.Length - 1))}\\Assets\\icon.ico");
            vm.StartPingingAll();
            _notifyIcon.OnTrayLeftMouseButtonClick += OnTrayIconLeftButtonClicked;
            _notifyIcon.OnTrayRightMouseButtonClick += OnTrayIconRightMouseButtonClick;
        }

        private void OnTrayIconRightMouseButtonClick()
        {
            CloseFormButton_Click(this, new RoutedEventArgs());
        }

        private void OnTrayIconLeftButtonClicked()
        {
            if(WindowState == WindowState.Minimized)
            {
                Show();
                WindowState = WindowState.Normal;
                _notifyIcon.Hide();
            }
        }

        private void DragField_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MinimizeFormButton_Click(object sender, RoutedEventArgs e)
        {
            _notifyIcon.Show();
            WindowState = WindowState.Minimized;
        }

        private void CloseFormButton_Click(object sender, RoutedEventArgs e)
        {
            _notifyIcon.Dispose();
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