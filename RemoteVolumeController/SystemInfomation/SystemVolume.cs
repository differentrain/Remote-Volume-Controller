using System;
using System.Runtime.InteropServices;
using RemoteVolumeController.SystemInfomation.NativeComponent;

namespace RemoteVolumeController.SystemInfomation
{
    public sealed class SystemVolume : IDisposable
    {

        private readonly Guid _mGuid;

        private readonly DefaultEndpointChangeClient _endpointChangedClient;
        private readonly EndpointVolumeChangeClient _endpointVolumeClient;

        private readonly IMMDeviceEnumerator _devices;
        private IAudioEndpointVolume _endpoint;

        private bool _isMuted;
        private float _masterVol;
        private uint _channelCount;
        private float[] _channelVols;

        public SystemVolume()
        {
            _devices = (IMMDeviceEnumerator)Activator.CreateInstance(Type.GetTypeFromCLSID(NativeConstants.MMDeviceEnumeratorCLSID));
            _mGuid = Guid.NewGuid();
            _endpointChangedClient = new DefaultEndpointChangeClient();
            _endpointChangedClient.OnDefaultEndpointChanged += EndpointChangedClient_OnDefaultEndpointChanged;
            _endpointVolumeClient = new EndpointVolumeChangeClient();
            _endpointVolumeClient.OnVolumeChanged += EndpointVolumeClient_OnVolumeChanged;
            Initialize(null);
        }

        public event EventHandler<bool> VolumeChanged;

        public bool Mute
        {
            get => _isMuted;
            set
            {
                 if (_endpoint.SetMute(value, _mGuid) < 0) throw new InvalidOperationException();
            }
        }

        public int ChannelCount => (int)_channelCount;

        public float MasterVolume
        {
            get => _masterVol;
            set
            {
                if (_masterVol < 0 || _masterVol > 1) throw new ArgumentOutOfRangeException();
                if (_endpoint.SetMasterVolumeLevelScalar(value, _mGuid) < 0) throw new InvalidOperationException();
            }
        }

        public float this[int channelIndex]
        {
            get => _channelVols[channelIndex];
            set
            {
                if (_endpoint.SetChannelVolumeLevelScalar((uint)channelIndex, value, _mGuid) < 0) throw new InvalidOperationException();
            }

        }

        private void EndpointVolumeClient_OnVolumeChanged(object sender, EndpointVolumeChangedEventArgs e)
        {
            _isMuted = e.Muted;
            _masterVol = e.MasterVolume;
            _channelCount = e.Channels;
            _channelVols = e.ChannelVolumes;
            VolumeChanged?.Invoke(this, e.GuidEventContext != _mGuid);
        }

        private void EndpointChangedClient_OnDefaultEndpointChanged(object sender, string e)
        {
            Release();
            Initialize(e);
        }

        private void Initialize(string deviceID = null)
        {
            disposedValue = true;

            IMMDevice defaultDevice;
            if (deviceID == null)
            {
                if (_devices.GetDefaultAudioEndpoint(
                    NativeConstants.EDataFlow_eRender,
                    NativeConstants.ERole_eMultimedia,
                    out defaultDevice) < 0)
                {
                    ThrowInvalid();
                }
            }
            else if (_devices.GetDevice(deviceID, out defaultDevice) < 0)
            {
                ThrowInvalid();
            }

            if (defaultDevice.Activate(
                NativeConstants.IAudioEndpointVolumeCLSID,
                NativeConstants.CLSCTX_ALL,
                IntPtr.Zero,
                out _endpoint) < 0)
            {
                Marshal.ReleaseComObject(defaultDevice);
                ThrowInvalid();
            }
            Marshal.ReleaseComObject(defaultDevice);


            if (_endpoint.GetMute(out _isMuted) < 0 ||
                _endpoint.GetMasterVolumeLevelScalar(out _masterVol) < 0 ||
                _endpoint.GetChannelCount(out _channelCount) < 0)
            {
                ThrowInvalidAll();
            }

            _channelVols = new float[_channelCount];

            unsafe
            {
                fixed (float* pf = _channelVols)
                {
                    for (uint i = 0; i < _channelCount; i++)
                    {
                        if (_endpoint.GetChannelVolumeLevelScalar(i, out pf[i]) < 0) ThrowInvalidAll();
                    }
                }
            }

            disposedValue = false;

            _devices.RegisterEndpointNotificationCallback(_endpointChangedClient);
            _endpoint.RegisterControlChangeNotify(_endpointVolumeClient);

            if (deviceID != null) VolumeChanged?.Invoke(this, default);


            void ThrowInvalid()
            {
                Marshal.ReleaseComObject(_devices);
                throw new InvalidOperationException();
            }

            void ThrowInvalidAll()
            {
                Marshal.ReleaseComObject(_endpoint);
                ThrowInvalid();
            }
        }

        private void Release()
        {
            try
            {
                _devices.UnregisterEndpointNotificationCallback(_endpointChangedClient);
                _endpoint.UnregisterControlChangeNotify(_endpointVolumeClient);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { }
#pragma warning restore CA1031 // Do not catch general exception types
            Marshal.ReleaseComObject(_endpoint);
            Marshal.ReleaseComObject(_devices);
        }



        #region IDisposable Support
        private bool disposedValue = false;


        private void DisposeCore()
        {
            if (!disposedValue)
            {
                Release();
                _endpointChangedClient.OnDefaultEndpointChanged -= EndpointChangedClient_OnDefaultEndpointChanged;
                _endpointVolumeClient.OnVolumeChanged -= EndpointVolumeClient_OnVolumeChanged;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            DisposeCore();
            GC.SuppressFinalize(this);
        }
        ~SystemVolume() => DisposeCore();


        #endregion
    }
}
