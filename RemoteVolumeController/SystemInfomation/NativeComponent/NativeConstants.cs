using System;

namespace RemoteVolumeController.SystemInfomation.NativeComponent
{
    internal static class NativeConstants
    {
        public const int GUID_SIZE = 16;
        public const int EDataFlow_eRender = 0;
        public const int ERole_eMultimedia = 1;
        public const int CLSCTX_ALL = 0x01 | 0x02 | 0x04 | 0x10;
        public static readonly Guid MMDeviceEnumeratorCLSID = new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E");
        public static readonly Guid IAudioEndpointVolumeCLSID = typeof(IAudioEndpointVolume).GUID;
    }
}
