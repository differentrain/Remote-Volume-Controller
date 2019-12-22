using System;
using System.Runtime.InteropServices;


namespace RemoteVolumeController.SystemInfomation.NativeComponent
{
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioEndpointVolume
    {
        [PreserveSig]
        int RegisterControlChangeNotify([In] IAudioEndpointVolumeCallback pNotify);

        [PreserveSig]
        int UnregisterControlChangeNotify([In] IAudioEndpointVolumeCallback pNotify);

        [PreserveSig]
        int GetChannelCount(
            [MarshalAs(UnmanagedType.U4), Out] out uint channelCount);

        int Reserve1();

        [PreserveSig]
        int SetMasterVolumeLevelScalar(
            [MarshalAs(UnmanagedType.R4), In] float level,
            [MarshalAs(UnmanagedType.LPStruct), In]Guid eventContext);

        int Reserve2();

        [PreserveSig]
        int GetMasterVolumeLevelScalar(
            [MarshalAs(UnmanagedType.R4), Out] out float level);

        int Reserve3();

        [PreserveSig]
        int SetChannelVolumeLevelScalar(
            [MarshalAs(UnmanagedType.U4), In] uint channelNumber,
            [MarshalAs(UnmanagedType.R4), In] float level,
            [MarshalAs(UnmanagedType.LPStruct), In] Guid eventContext);

        int Reserve4();

        [PreserveSig]
        int GetChannelVolumeLevelScalar(
            [MarshalAs(UnmanagedType.U4), In] uint channelNumber,
            [MarshalAs(UnmanagedType.R4), Out] out float level);

        [PreserveSig]
        int SetMute(
            [MarshalAs(UnmanagedType.Bool), In] bool isMuted,
            [MarshalAs(UnmanagedType.LPStruct), In] Guid eventContext);

        [PreserveSig]
        int GetMute(
            [MarshalAs(UnmanagedType.Bool), Out] out bool isMuted);
    }
}
