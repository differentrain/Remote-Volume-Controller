using RemoteVolumeController.SystemInfomation;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RemoteVolumeController.RemoteVolumeControllerService
{
    public sealed class HttpVolumeControllerServer : IDisposable
    {

        private readonly HttpListener _listener;
        private readonly SystemVolume _sysVol;

        private readonly int _port;

        private readonly string _cmdArgAddUrl;
        private readonly string _cmdArgDelUrl;

        private HttpVolumeControllerServer(int port, SystemVolume sysVol)
        {
            _listener = new HttpListener();

            var serAddr = $"http://+:{port.ToString()}/volume/";
            _cmdArgAddUrl = $"http add urlacl url={serAddr} user={WindowsIdentity.GetCurrent().Name}";
            _cmdArgDelUrl = $"http delete urlacl url={serAddr}";

            _listener.Prefixes.Add(serAddr);
            _sysVol = sysVol;
            ServerAddress = $"http://{Utilities.GetLocalIP()}:{port}/volume";
            _port = port;
        }

        public string ServerAddress { get; }

        public void Start()
        {
            if (_listener.IsListening) return;

            NetshSetSet(_cmdArgAddUrl, CmdArgAddPort + _port.ToString());
            _listener.Start();

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (true) await ListenFunction(_listener).ConfigureAwait(false);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch { }
#pragma warning restore CA1031 // Do not catch general exception types
            }, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            if (!_listener.IsListening) return;
            NetshSetSet(_cmdArgDelUrl, CmdArgDelPort + _port.ToString());
            _listener.Stop();
        }

        private async Task ListenFunction(HttpListener l)
        {
            //try
            //{
            var context = await l.GetContextAsync().ConfigureAwait(false);
            var request = context.Request;

            var response = context.Response;

            if (request.HttpMethod != "GET")
            {
                response.Close();
                return;
            }
            string responseString;

            if (request.RawUrl == "/volume/" || request.RawUrl == "/volume")
            {
                responseString = Utilities.HtmpTempl;
            }
            else if (request.RawUrl.StartsWith("/volume/api/getinfo"))
            {
                responseString = $@"{{""mute"":{_sysVol.Mute.ToString().ToLower()},""vol"":{Math.Ceiling(_sysVol.MasterVolume * 100)}}}";
            }
            else if (request.RawUrl.StartsWith("/volume/api/setinfo?"))
            {
                var muteStr = request.QueryString.Get("mute");
                if (muteStr != null && bool.TryParse(muteStr, out var mute)) _sysVol.Mute = mute;
                var volStr = request.QueryString.Get("vol");
                if (volStr != null && int.TryParse(volStr, out var vol)) _sysVol.MasterVolume = vol / 100f;
                response.Close();
                return;
            }
            else if (request.RawUrl.StartsWith("/volume/api/lockscreen"))
            {
                NativeMethods.LockWorkStation();
                response.Close();
                return;
            }
            else if (request.RawUrl.Length >= 22 && request.RawUrl.StartsWith("/volume/api/foobar"))
            {
                switch (request.RawUrl[20])
                {
                    case 'x': // next
                        await BeefwebClient.Next();
                        break;
                    case 'e': // previous
                        await BeefwebClient.Previous();
                        break;
                    case 'a': // start
                        await BeefwebClient.Play();
                        break;
                    case 'o': // stop
                        await BeefwebClient.Stop();
                        break;
                    case 'g': // toggle
                        await BeefwebClient.Toggle();
                        break;
                }
                response.Close();
                return;
            }
            else
            {
                response.Close();
                return;
            }
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using (var output = response.OutputStream)
                await output.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }




        #region static members
        private const int MIN_DYANMIC_PORT = 49152;

        private static HttpVolumeControllerServer _server = null;

        public static HttpVolumeControllerServer GetOrCreateServer(SystemVolume sysVol) => GetOrCreateServer(MIN_DYANMIC_PORT, sysVol);

        public static HttpVolumeControllerServer GetOrCreateServer(int minPort, SystemVolume sysVol)
        {
            if (minPort < 1024 || minPort > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(minPort));
            if (_server != null && !_server.disposedValue) return _server;
            _server = new HttpVolumeControllerServer(GetAvailablePort(minPort), sysVol);
            return _server;
        }



        private static int GetAvailablePort(int startingPort)
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var x = properties.GetActiveTcpConnections()
                 .Where(c => c.LocalEndPoint.Port >= startingPort)
                 .Select(c => c.LocalEndPoint.Port)
                 .Concat(
                     properties.GetActiveTcpListeners()
                         .Where(t => t.Port >= startingPort)
                         .Select(t => t.Port))
                 .Concat(
                     properties.GetActiveUdpListeners()
                         .Where(u => u.Port >= startingPort)
                         .Select(u => u.Port)
                 ).OrderBy(p => p);

            foreach (var item in x)
                if (item == startingPort) ++startingPort;
            return startingPort;
        }
        private static readonly ProcessStartInfo cmdStartInfo = new ProcessStartInfo()
        {
            FileName = "netsh",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            Verb = "runas"
        };
        private const string CmdArgAddPort = @"advfirewall firewall add rule name=""RemoteVolumeControl10275915-be49-4291-86a1-c2622c7146fe"" dir=in action=allow protocol=TCP localport=";
        private const string CmdArgDelPort = @"advfirewall firewall delete rule name=""RemoteVolumeControl10275915-be49-4291-86a1-c2622c7146fe"" protocol=TCP localport=";

        private static void NetshSetSet(params string[] args)
        {
            for (int i = 0; i < 2; i++)
            {
                cmdStartInfo.Arguments = args[i];
                using (var p = Process.Start(cmdStartInfo))
                {
                    p.WaitForExit();
                }
            }

        }


        #region IDisposable Support
        private bool disposedValue = false;
        public void Dispose()
        {
            if (!disposedValue)
            {
                NetshSetSet(_cmdArgDelUrl, CmdArgDelPort + _port.ToString());
                _listener.Abort();
                _listener.Close();
                disposedValue = true;
            }
        }
        #endregion



        #endregion




    }
}
