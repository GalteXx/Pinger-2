using System.Runtime.InteropServices;

namespace Pinger_2.WinApi
{
    //I don't need an abstraction layer
    //app is windows only anyway
    internal class NotifyWindow
    {
        private NOTIFYICONDATA _notifyIconData;
        public NotifyWindow()
        {
            _notifyIconData = new()
            {
                uID = 1,
                uFlags = 0x00000001 | 0x00000002 //NIF_MESSAGE | NIF_ICON
            };
            _notifyIconData.cbSize = Marshal.SizeOf(_notifyIconData);
        }

        public void Show()
        {
            Shell_NotifyIcon(0x00000000, ref _notifyIconData);
        }

        [DllImport("shell32.dll")]
        private static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpdata);
    }
}
