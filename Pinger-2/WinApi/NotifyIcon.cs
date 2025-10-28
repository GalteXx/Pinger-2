using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Pinger_2.WinApi
{
    //I don't need an abstraction layer
    //app is windows only anyway
    internal partial class NotifyIcon : IDisposable
    {
        private const int NIM_ADD = 0x00000000;
        private const int NIM_MODIFY = 0x00000001;
        private const int NIM_DELETE = 0x00000002;
        private const int NIF_MESSAGE = 0x00000001;
        private const int NIF_ICON = 0x00000002;
        private const int NIF_TIP = 0x00000004;
        private const int WM_TRAYICON = 0x800;
        private const uint IMAGE_ICON = 1;
        private const uint LR_LOADFROMFILE = 0x00000010;
        private const uint LR_DEFAULTSIZE = 0x00000040;

        private readonly IntPtr _windowHandle;
        private readonly HwndSource _hwndSource;

        public event Action OnTrayLeftMouseButtonClick;
        public event Action OnTrayRightMouseButtonClick;


        private NOTIFYICONDATA _notifyIconData;
        public NotifyIcon(Window window, string iconPath)
        {
            OnTrayLeftMouseButtonClick += () => { };
            OnTrayRightMouseButtonClick += () => { };

            _windowHandle = new WindowInteropHelper(window).EnsureHandle();
            _hwndSource = HwndSource.FromHwnd(_windowHandle);
            _hwndSource.AddHook(WndProc);
            var ico = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 0, 0, LR_LOADFROMFILE | LR_DEFAULTSIZE);
            if (ico == IntPtr.Zero)
            {
                throw new Exception($"The Icon could not be loaded with WinApi error: {Marshal.GetLastWin32Error()}");
            }

            _notifyIconData = new()
            {
                uID = 1,
                hWnd = _windowHandle,
                uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP,
                uCallbackMessage = WM_TRAYICON,
                szTip = "Placeholder",
                hIcon = ico
            };
            _notifyIconData.cbSize = Marshal.SizeOf(_notifyIconData);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_TRAYICON)
            {
                switch (lParam.ToInt32())
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

        public bool Show()
        {
            return Shell_NotifyIcon(NIM_ADD, ref _notifyIconData);
        }

        public bool Hide()
        {
            return Shell_NotifyIcon(NIM_DELETE, ref _notifyIconData);
        }

        [DllImport("shell32.dll")]
        private static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpdata);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

        public void Dispose()
        {
            Shell_NotifyIcon(NIM_DELETE, ref _notifyIconData);
            _hwndSource.RemoveHook(WndProc);
        }
    }
}
