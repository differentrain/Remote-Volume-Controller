using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RemoteVolumeController.RemoteVolumeControllerService
{
    public static class BeefwebClient
    {

        public const int DefaultPort = 8880;

        private static readonly HttpClient _client = new HttpClient() { Timeout = TimeSpan.FromSeconds(2) };

        private static  string _apiUrl = $"http://{Utilities.GetLocalIP()}:{DefaultPort}/api/player/";

        private static int _port = DefaultPort;

        public static int Port
        {
            get => _port;
            set
            {
                if (value < 1024 || value > ushort.MaxValue) throw new ArgumentOutOfRangeException();
                if (_port == value) return;
                _port = value;
                _apiUrl = $"http://{Utilities.GetLocalIP()}:{_port}/api/player/";
            }
        }

        public static async Task Previous() => await PostApi("previous");

        public static async Task Next() => await PostApi("next");

        public static async Task Play() => await PostApi("play");

        public static async Task Stop() => await PostApi("stop");

        public static async Task Toggle() => await PostApi("pause/toggle");


        private static async Task PostApi(string api)
        {
            try
            {
                await _client.PostAsync(_apiUrl + api, null);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { }
#pragma warning restore CA1031 // Do not catch general exception types
        }

    }
}
