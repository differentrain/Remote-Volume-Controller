using System;

namespace RemoteVolumeController
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Utilities.RunApp<FormMain>("10275915-be49-4291-86a1-c2622c7146fe");
        }
    }
}
