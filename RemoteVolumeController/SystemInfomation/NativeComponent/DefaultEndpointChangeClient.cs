using System;
using System.Runtime.InteropServices;


namespace RemoteVolumeController.SystemInfomation.NativeComponent
{
    [Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
    internal sealed class DefaultEndpointChangeClient : IMMNotificationClient
    {
        public event EventHandler<string> OnDefaultEndpointChanged;

        int IMMNotificationClient.Reserve1() => throw new NotImplementedException();
        int IMMNotificationClient.Reserve2() => throw new NotImplementedException();
        int IMMNotificationClient.Reserve3() => throw new NotImplementedException();
        int IMMNotificationClient.OnDefaultDeviceChanged(int dataFlow, int role, string pwstrDefaultDeviceId)
        {
            if (string.IsNullOrEmpty(pwstrDefaultDeviceId)) throw new InvalidComObjectException();
            if (dataFlow == NativeConstants.EDataFlow_eRender &&
                role == NativeConstants.ERole_eMultimedia)
            {
                OnDefaultEndpointChanged?.Invoke(this, pwstrDefaultDeviceId);
            }
            return 0;
        }
    }
}
