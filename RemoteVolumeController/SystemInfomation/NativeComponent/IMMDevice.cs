using System;
using System.Runtime.InteropServices;


namespace RemoteVolumeController.SystemInfomation.NativeComponent
{
    [ComImport]
    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDevice
    {
        [PreserveSig]
        int Activate(
            [MarshalAs(UnmanagedType.LPStruct), In] Guid iid,
            [In] int dwClsCtx,
            [In] IntPtr pActivationParams,
            [Out] out IAudioEndpointVolume ppInterface);
    }
}
