using System;
using System.Runtime.InteropServices;


namespace RemoteVolumeController.SystemInfomation.NativeComponent
{
    [ComImport]
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        int Reserve1();

        [PreserveSig]
        int GetDefaultAudioEndpoint(
            [In] int dataFlow,
            [In] int role,
            [Out] out IMMDevice ppDevice);

        [PreserveSig]
        int GetDevice(
            [MarshalAs(UnmanagedType.LPWStr), In] string pwstrId,
            [Out] out IMMDevice ppDevice);

        [PreserveSig]
        int RegisterEndpointNotificationCallback(
            [In] IMMNotificationClient pClient);

        [PreserveSig]
        int UnregisterEndpointNotificationCallback(
            [In] IMMNotificationClient pClient);
    }
}
