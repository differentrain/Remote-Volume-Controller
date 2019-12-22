using System;
using System.Runtime.InteropServices;


namespace RemoteVolumeController.SystemInfomation.NativeComponent
{

    internal sealed class EndpointVolumeChangedEventArgs : EventArgs
    {

        public readonly Guid GuidEventContext;
        public readonly bool Muted;
        public readonly float MasterVolume;
        public readonly uint Channels;
        public readonly float[] ChannelVolumes;


        public EndpointVolumeChangedEventArgs(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) throw new InvalidComObjectException();
            unsafe
            {
                GuidEventContext = *(Guid*)ptr;
                ptr += NativeConstants.GUID_SIZE;
                Muted = *(bool*)ptr;
                MasterVolume = *(float*)(ptr + 4);
                Channels = *(uint*)(ptr + 8);
                ChannelVolumes = new float[Channels];
                fixed (float* pf = ChannelVolumes)
                {
                    for (int i = 0; i < Channels; i++)
                    {
                        pf[i] = *(float*)(ptr + 12 + i * 4);
                    }
                }
            }
            Marshal.Release(ptr);
        }



    }
}
