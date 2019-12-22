using System.Runtime.InteropServices;

namespace RemoteVolumeController.RemoteVolumeControllerService
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = false)]
        public static extern bool LockWorkStation();
    }
}
