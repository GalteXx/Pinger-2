using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Pinger_2.WinApi
{
    //I don't need an abstraction layer
    //app is windows only anyway
    internal class NotifyWindow : IDisposable
    {
        private const int NIM_ADD = 0x00000000;
        private const int NIM_MODIFY = 0x00000001;
        private const int NIM_DELETE = 0x00000002;
        private const int NIF_MESSAGE = 0x00000001;
        private const int NIF_ICON = 0x00000002;
        private const int WM_TRAYICON = 0x800;
        private readonly IntPtr _windowHandle;
        private readonly HwndSource _hwndSource;

        public event Action OnTrayLeftMouseButtonClick;
        public event Action OnTrayRightMouseButtonClick;

        private NOTIFYICONDATA _notifyIconData;
        public NotifyWindow(Window window, string tooltip)
        {
            OnTrayLeftMouseButtonClick += () => { };
            OnTrayRightMouseButtonClick += () => { };

            _windowHandle = new WindowInteropHelper(window).EnsureHandle();
            _hwndSource = HwndSource.FromHwnd(_windowHandle);
            _hwndSource.AddHook(WndProc);

            _notifyIconData = new()
            {
                uID = 1,
                uFlags = NIF_MESSAGE | NIF_ICON,
                uCallbackMessage = WM_TRAYICON,
                //Icon
            };
            _notifyIconData.cbSize = Marshal.SizeOf(_notifyIconData);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_TRAYICON)
            {
                int eventId = lParam.ToInt32();

                switch (eventId)
                {
                    case 0x201: // WM_LBUTTONDOWN
                        OnLeftClick();
                        break;
                    case 0x204: // WM_RBUTTONDOWN
                        OnRightClick();
                        break;
                }
            }

            return IntPtr.Zero;
        }

        private void OnLeftClick()
        {
            OnTrayLeftMouseButtonClick.Invoke();
        }

        private void OnRightClick()
        {
            OnTrayRightMouseButtonClick.Invoke();
        }

        public void Show()
        {
            Shell_NotifyIcon(NIM_ADD, ref _notifyIconData);
        }

        [DllImport("shell32.dll")]
        private static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpdata);

        public void Dispose()
        {
            Shell_NotifyIcon(NIM_DELETE, ref _notifyIconData);
            _hwndSource.RemoveHook(WndProc);
        }
    }
}
