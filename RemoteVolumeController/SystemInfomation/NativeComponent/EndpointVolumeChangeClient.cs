using System;
using System.Runtime.InteropServices;


namespace RemoteVolumeController.SystemInfomation.NativeComponent
{
    [Guid("657804FA-D6AD-4496-8A60-352752AF4F89")]
    internal sealed class EndpointVolumeChangeClient : IAudioEndpointVolumeCallback
    {
        public event EventHandler<EndpointVolumeChangedEventArgs> OnVolumeChanged;


        int IAudioEndpointVolumeCallback.OnNotify(IntPtr pNotify)
        {
            OnVolumeChanged?.Invoke(this,new EndpointVolumeChangedEventArgs(pNotify));
            return 0;
        }
    }
}
