using System;
using System.Runtime.InteropServices;

namespace RemoteVolumeController.SystemInfomation.NativeComponent
{
    [ComImport]
    [Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMNotificationClient
    {
        int Reserve1();
        int Reserve2();
        int Reserve3();

        [PreserveSig]
        int OnDefaultDeviceChanged([In] int dataFlow, [In] int role, [MarshalAs(UnmanagedType.LPWStr), In] string pwstrDefaultDeviceId);
    }
}
