using Microsoft.Win32;
using RemoteVolumeController.RemoteVolumeControllerService;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RemoteVolumeController
{
    internal static class Utilities
    {
        private static readonly string _savefilePath = Application.StartupPath + @"\RemoteVolumeController.config";
        private static readonly string _errorLogPath = Application.StartupPath + @"\RemoteVolumeController.log";

        public static readonly string StrTitle;
        public static readonly string StrStartServer;
        public static readonly string StrAutoRunApp;
        public static readonly string StrAutoRunServer;
        public static readonly string StrConfig;
        public static readonly string StrVolume;
        public static readonly string StrMute;
        public static readonly string StrOpenMainWindow;
        public static readonly string StrExit;
        public static readonly string StrLockScreen;
        public static readonly string StrRefresh;

        public static readonly string StrPlayer;
        public static readonly string StrPrevious;
        public static readonly string StrNext;
        public static readonly string StrStart;
        public static readonly string StrStop;
        public static readonly string StrToggle;
        public static readonly string StrFoobarPort;

        public static readonly string DefaultServerName;
        public static readonly string HtmpTempl;

        private static bool _autoRunApp = false;
        private static bool _autoRunServer = false;
        private static uint _port = 8880;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static Utilities()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            if (File.Exists(_savefilePath))
            {
                using (var fr = new FileStream(_savefilePath, FileMode.Open, FileAccess.Read))
                {
                    if (fr.Length != 3)
                    {
                        fr.Dispose();
                        File.Delete(_savefilePath);
                        WriteConfig();
                        return;
                    }

                  
                        _autoRunApp = File.Exists(_myAutoRunPath);
                        if (_autoRunApp) SetShortcut(true);
                  

                    var buf = new byte[3];
                    fr.Read(buf, 0, 3);
                    _autoRunServer = buf[0] == 1;
                    _port = BitConverter.ToUInt16(buf, 1);
                    BeefwebClient.Port = (int)_port;
                }
            }
            else
            {
                SetShortcut(false);
                WriteConfig();
            }

            if (CultureInfo.InstalledUICulture.ThreeLetterISOLanguageName == "zho")
            {
                StrTitle = "远程音量控制";
                StrConfig = "设置";
                StrStartServer = "启用服务器";
                StrAutoRunApp = "开机自动运行";
                StrAutoRunServer = "自动启用服务器";
                StrVolume = "音量";
                StrMute = "静音";
                StrOpenMainWindow = "打开主窗口(&M)";
                StrExit = "退出(&Q)";
                StrLockScreen = "锁定屏幕";
                StrRefresh = "刷新";

                StrPlayer = "播放器";
                StrPrevious = "前一首";
                StrNext = "下一首";
                StrStart = "播放";
                StrStop = "停止";
                StrToggle = "切换";
                StrFoobarPort = "Beefweb(foobar远程控制组件)端口号：";
            }
            else
            {
                StrTitle = "Remote Volume Controll";
                StrConfig = "Config";
                StrStartServer = "Enable Server";
                StrAutoRunApp = "Auto Run At Startup";
                StrAutoRunServer = "Auto Enable Server";
                StrVolume = "Vol";
                StrMute = "Mute";
                StrOpenMainWindow = "Open Main Window(&M)";
                StrExit = "Quit(&Q)";
                StrLockScreen = "Lock Screen";
                StrRefresh = "Refresh";
                StrPlayer = "player";
                StrPrevious = "Previous";
                StrNext = "Next";
                StrStart = "Play";
                StrStop = "Stop";
                StrToggle = "Toggle";
                StrFoobarPort = "Beefweb(foobar remote control component) Port";
            }

            DefaultServerName = Dns.GetHostName() + $" {StrVolume}";
            HtmpTempl = Encoding.UTF8.GetString(
                Convert.FromBase64String(
                   "PCFET0NUWVBFIGh0bWw+PGh0bWw+PGhlYWQ+PG1ldGEgbmFtZT0idmlld3BvcnQiIGNvbnRlbnQ9IndpZHRoPWRldmljZS13aWR0aCwgaW5pdGlhbC1zY2FsZT0xIj48bWV0YSBjaGFyc2V0PSJVVEYtOCI+PHRpdGxlPiVTZXJ2ZXJOYW1lJTwvdGl0bGU+PHN0eWxlPi5zbGlkZXJ7LXdlYmtpdC1hcHBlYXJhbmNlOm5vbmU7d2lkdGg6OTglO2hlaWdodDoyNXB4O2JhY2tncm91bmQ6I2QzZDNkMztvdXRsaW5lOjA7b3BhY2l0eTouNzstd2Via2l0LXRyYW5zaXRpb246LjJzO3RyYW5zaXRpb246b3BhY2l0eSAuMnN9LnNsaWRlcjpob3ZlcntvcGFjaXR5OjF9LnNsaWRlcjo6LXdlYmtpdC1zbGlkZXItdGh1bWJ7LXdlYmtpdC1hcHBlYXJhbmNlOm5vbmU7YXBwZWFyYW5jZTpub25lO3dpZHRoOjI1cHg7aGVpZ2h0OjI1cHg7YmFja2dyb3VuZDojNGNhZjUwO2N1cnNvcjpwb2ludGVyfS5zbGlkZXI6Oi1tb3otcmFuZ2UtdGh1bWJ7d2lkdGg6MjVweDtoZWlnaHQ6MjVweDtiYWNrZ3JvdW5kOiM0Y2FmNTA7Y3Vyc29yOnBvaW50ZXJ9PC9zdHlsZT48c2NyaXB0IHR5cGU9InRleHQvamF2YXNjcmlwdCI+d2luZG93Lm9ubG9hZD1mdW5jdGlvbigpe2dldEluZm8oKTt2b2w9ZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoIm15UmFuZ2UiKTtjb25zdCByYW5nZXM9UmFuZ2VUb3VjaC5zZXR1cCh2b2wpfTtmdW5jdGlvbiBnZXRJbmZvKCl7dm9sPWRvY3VtZW50LmdldEVsZW1lbnRCeUlkKCJteVJhbmdlIik7dHh0PWRvY3VtZW50LmdldEVsZW1lbnRCeUlkKCJteVRleHQiKTttdXRlPWRvY3VtZW50LmdldEVsZW1lbnRCeUlkKCJteU11dGUiKTt2YXIgeG1saHR0cD1uZXcgWE1MSHR0cFJlcXVlc3QoKTt4bWxodHRwLm9ucmVhZHlzdGF0ZWNoYW5nZT1mdW5jdGlvbigpe2lmKHhtbGh0dHAucmVhZHlTdGF0ZT09NCYmeG1saHR0cC5zdGF0dXM9PTIwMCl7dmFyIGpzb249SlNPTi5wYXJzZSh4bWxodHRwLnJlc3BvbnNlVGV4dCk7bXV0ZS5jaGVja2VkPWpzb24ubXV0ZTt2b2wudmFsdWU9anNvbi52b2w7dHh0LmlubmVySFRNTD0iJVZvbCXvvJoiK3ZvbC52YWx1ZX19O3htbGh0dHAub3BlbigiR0VUIiwiL3ZvbHVtZS9hcGkvZ2V0aW5mbyIsZmFsc2UpO3htbGh0dHAuc2VuZCgpfWZ1bmN0aW9uIGxvY2tTY3JlZW4oKXt2YXIgeG1saHR0cD1uZXcgWE1MSHR0cFJlcXVlc3QoKTt4bWxodHRwLm9wZW4oIkdFVCIsIi92b2x1bWUvYXBpL2xvY2tzY3JlZW4iLGZhbHNlKTt4bWxodHRwLnNlbmQoKX1mdW5jdGlvbiBmb29iYXJOZXh0KCl7dmFyIHhtbGh0dHA9bmV3IFhNTEh0dHBSZXF1ZXN0KCk7eG1saHR0cC5vcGVuKCJHRVQiLCIvdm9sdW1lL2FwaS9mb29iYXJOZXh0IixmYWxzZSk7eG1saHR0cC5zZW5kKCl9ZnVuY3Rpb24gZm9vYmFyUHJldmlvdXMoKXt2YXIgeG1saHR0cD1uZXcgWE1MSHR0cFJlcXVlc3QoKTt4bWxodHRwLm9wZW4oIkdFVCIsIi92b2x1bWUvYXBpL2Zvb2JhclByZXZpb3VzIixmYWxzZSk7eG1saHR0cC5zZW5kKCl9ZnVuY3Rpb24gZm9vYmFyU3RhcnQoKXt2YXIgeG1saHR0cD1uZXcgWE1MSHR0cFJlcXVlc3QoKTt4bWxodHRwLm9wZW4oIkdFVCIsIi92b2x1bWUvYXBpL2Zvb2JhclN0YXJ0IixmYWxzZSk7eG1saHR0cC5zZW5kKCl9ZnVuY3Rpb24gZm9vYmFyU3RvcCgpe3ZhciB4bWxodHRwPW5ldyBYTUxIdHRwUmVxdWVzdCgpO3htbGh0dHAub3BlbigiR0VUIiwiL3ZvbHVtZS9hcGkvZm9vYmFyU3RvcCIsZmFsc2UpO3htbGh0dHAuc2VuZCgpfWZ1bmN0aW9uIGZvb2JhclRvZ2dsZSgpe3ZhciB4bWxodHRwPW5ldyBYTUxIdHRwUmVxdWVzdCgpO3htbGh0dHAub3BlbigiR0VUIiwiL3ZvbHVtZS9hcGkvZm9vYmFyVG9nZ2xlIixmYWxzZSk7eG1saHR0cC5zZW5kKCl9ZnVuY3Rpb24gc2V0SW5mbygpe3ZhciB4bWxodHRwPW5ldyBYTUxIdHRwUmVxdWVzdCgpO3ZvbD1kb2N1bWVudC5nZXRFbGVtZW50QnlJZCgibXlSYW5nZSIpO3R4dD1kb2N1bWVudC5nZXRFbGVtZW50QnlJZCgibXlUZXh0Iik7dHh0LmlubmVySFRNTD0iJVZvbCXvvJoiK3ZvbC52YWx1ZTttdXRlPWRvY3VtZW50LmdldEVsZW1lbnRCeUlkKCJteU11dGUiKTt4bWxodHRwLm9wZW4oIkdFVCIsIi92b2x1bWUvYXBpL3NldGluZm8/IisibXV0ZT0iK211dGUuY2hlY2tlZCsiJnZvbD0iK3ZvbC52YWx1ZSxmYWxzZSk7eG1saHR0cC5zZW5kKCl9OyFmdW5jdGlvbihlLHQpeyJvYmplY3QiPT10eXBlb2YgZXhwb3J0cyYmInVuZGVmaW5lZCIhPXR5cGVvZiBtb2R1bGU/bW9kdWxlLmV4cG9ydHM9dCgpOiJmdW5jdGlvbiI9PXR5cGVvZiBkZWZpbmUmJmRlZmluZS5hbWQ/ZGVmaW5lKCJSYW5nZVRvdWNoIix0KTplLlJhbmdlVG91Y2g9dCgpfSh0aGlzLGZ1bmN0aW9uKCl7InVzZSBzdHJpY3QiO2Z1bmN0aW9uIGUoZSx0KXtmb3IodmFyIG49MDtuPHQubGVuZ3RoO24rKyl7dmFyIHI9dFtuXTtyLmVudW1lcmFibGU9ci5lbnVtZXJhYmxlfHwhMSxyLmNvbmZpZ3VyYWJsZT0hMCwidmFsdWUiaW4gciYmKHIud3JpdGFibGU9ITApLE9iamVjdC5kZWZpbmVQcm9wZXJ0eShlLHIua2V5LHIpfX12YXIgdD17YWRkQ1NTOiEwLHRodW1iV2lkdGg6MTUsd2F0Y2g6ITB9O3ZhciBuPWZ1bmN0aW9uKGUpe3JldHVybiBudWxsIT1lP2UuY29uc3RydWN0b3I6bnVsbH0scj1mdW5jdGlvbihlLHQpe3JldHVybiEhKGUmJnQmJmUgaW5zdGFuY2VvZiB0KX0sdT1mdW5jdGlvbihlKXtyZXR1cm4gbnVsbD09ZX0saT1mdW5jdGlvbihlKXtyZXR1cm4gbihlKT09PU9iamVjdH0sbz1mdW5jdGlvbihlKXtyZXR1cm4gbihlKT09PVN0cmluZ30sYT1mdW5jdGlvbihlKXtyZXR1cm4gQXJyYXkuaXNBcnJheShlKX0sYz1mdW5jdGlvbihlKXtyZXR1cm4gcihlLE5vZGVMaXN0KX0sbD17bnVsbE9yVW5kZWZpbmVkOnUsb2JqZWN0OmksbnVtYmVyOmZ1bmN0aW9uKGUpe3JldHVybiBuKGUpPT09TnVtYmVyJiYhTnVtYmVyLmlzTmFOKGUpfSxzdHJpbmc6byxib29sZWFuOmZ1bmN0aW9uKGUpe3JldHVybiBuKGUpPT09Qm9vbGVhbn0sZnVuY3Rpb246ZnVuY3Rpb24oZSl7cmV0dXJuIG4oZSk9PT1GdW5jdGlvbn0sYXJyYXk6YSxub2RlTGlzdDpjLGVsZW1lbnQ6ZnVuY3Rpb24oZSl7cmV0dXJuIHIoZSxFbGVtZW50KX0sZXZlbnQ6ZnVuY3Rpb24oZSl7cmV0dXJuIHIoZSxFdmVudCl9LGVtcHR5OmZ1bmN0aW9uKGUpe3JldHVybiB1KGUpfHwobyhlKXx8YShlKXx8YyhlKSkmJiFlLmxlbmd0aHx8aShlKSYmIU9iamVjdC5rZXlzKGUpLmxlbmd0aH19O2Z1bmN0aW9uIHMoZSx0KXtpZigxPnQpe3ZhciBuPWZ1bmN0aW9uKGUpe3ZhciB0PSIiLmNvbmNhdChlKS5tYXRjaCgvKD86XC4oXGQrKSk/KD86W2VFXShbKy1dP1xkKykpPyQvKTtyZXR1cm4gdD9NYXRoLm1heCgwLCh0WzFdP3RbMV0ubGVuZ3RoOjApLSh0WzJdPyt0WzJdOjApKTowfSh0KTtyZXR1cm4gcGFyc2VGbG9hdChlLnRvRml4ZWQobikpfXJldHVybiBNYXRoLnJvdW5kKGUvdCkqdH1yZXR1cm4gZnVuY3Rpb24oKXtmdW5jdGlvbiBuKGUscil7KGZ1bmN0aW9uKGUsdCl7aWYoIShlIGluc3RhbmNlb2YgdCkpdGhyb3cgbmV3IFR5cGVFcnJvcigiQ2Fubm90IGNhbGwgYSBjbGFzcyBhcyBhIGZ1bmN0aW9uIil9KSh0aGlzLG4pLGwuZWxlbWVudChlKT90aGlzLmVsZW1lbnQ9ZTpsLnN0cmluZyhlKSYmKHRoaXMuZWxlbWVudD1kb2N1bWVudC5xdWVyeVNlbGVjdG9yKGUpKSxsLmVsZW1lbnQodGhpcy5lbGVtZW50KSYmbC5lbXB0eSh0aGlzLmVsZW1lbnQucmFuZ2VUb3VjaCkmJih0aGlzLmNvbmZpZz1PYmplY3QuYXNzaWduKHt9LHQsciksdGhpcy5pbml0KCkpfXJldHVybiByPW4saT1be2tleToic2V0dXAiLHZhbHVlOmZ1bmN0aW9uKGUpe3ZhciByPTE8YXJndW1lbnRzLmxlbmd0aCYmdm9pZCAwIT09YXJndW1lbnRzWzFdP2FyZ3VtZW50c1sxXTp7fSx1PW51bGw7aWYobC5lbXB0eShlKXx8bC5zdHJpbmcoZSk/dT1BcnJheS5mcm9tKGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwobC5zdHJpbmcoZSk/ZTonaW5wdXRbdHlwZT0icmFuZ2UiXScpKTpsLmVsZW1lbnQoZSk/dT1bZV06bC5ub2RlTGlzdChlKT91PUFycmF5LmZyb20oZSk6bC5hcnJheShlKSYmKHU9ZS5maWx0ZXIobC5lbGVtZW50KSksbC5lbXB0eSh1KSlyZXR1cm4gbnVsbDt2YXIgaT1PYmplY3QuYXNzaWduKHt9LHQscik7bC5zdHJpbmcoZSkmJmkud2F0Y2gmJm5ldyBNdXRhdGlvbk9ic2VydmVyKGZ1bmN0aW9uKHQpe0FycmF5LmZyb20odCkuZm9yRWFjaChmdW5jdGlvbih0KXtBcnJheS5mcm9tKHQuYWRkZWROb2RlcykuZm9yRWFjaChmdW5jdGlvbih0KXtsLmVsZW1lbnQodCkmJmZ1bmN0aW9uKGUsdCl7cmV0dXJuIGZ1bmN0aW9uKCl7cmV0dXJuIEFycmF5LmZyb20oZG9jdW1lbnQucXVlcnlTZWxlY3RvckFsbCh0KSkuaW5jbHVkZXModGhpcyl9LmNhbGwoZSx0KX0odCxlKSYmbmV3IG4odCxpKX0pfSl9KS5vYnNlcnZlKGRvY3VtZW50LmJvZHkse2NoaWxkTGlzdDohMCxzdWJ0cmVlOiEwfSk7cmV0dXJuIHUubWFwKGZ1bmN0aW9uKGUpe3JldHVybiBuZXcgbihlLHIpfSl9fSx7a2V5OiJlbmFibGVkIixnZXQ6ZnVuY3Rpb24oKXtyZXR1cm4ib250b3VjaHN0YXJ0ImluIGRvY3VtZW50LmRvY3VtZW50RWxlbWVudH19XSwodT1be2tleToiaW5pdCIsdmFsdWU6ZnVuY3Rpb24oKXtuLmVuYWJsZWQmJih0aGlzLmNvbmZpZy5hZGRDU1MmJih0aGlzLmVsZW1lbnQuc3R5bGUudXNlclNlbGVjdD0ibm9uZSIsdGhpcy5lbGVtZW50LnN0eWxlLndlYktpdFVzZXJTZWxlY3Q9Im5vbmUiLHRoaXMuZWxlbWVudC5zdHlsZS50b3VjaEFjdGlvbj0ibWFuaXB1bGF0aW9uIiksdGhpcy5saXN0ZW5lcnMoITApLHRoaXMuZWxlbWVudC5yYW5nZVRvdWNoPXRoaXMpfX0se2tleToiZGVzdHJveSIsdmFsdWU6ZnVuY3Rpb24oKXtuLmVuYWJsZWQmJih0aGlzLmxpc3RlbmVycyghMSksdGhpcy5lbGVtZW50LnJhbmdlVG91Y2g9bnVsbCl9fSx7a2V5OiJsaXN0ZW5lcnMiLHZhbHVlOmZ1bmN0aW9uKGUpe3ZhciB0PXRoaXMsbj1lPyJhZGRFdmVudExpc3RlbmVyIjoicmVtb3ZlRXZlbnRMaXN0ZW5lciI7WyJ0b3VjaHN0YXJ0IiwidG91Y2htb3ZlIiwidG91Y2hlbmQiXS5mb3JFYWNoKGZ1bmN0aW9uKGUpe3QuZWxlbWVudFtuXShlLGZ1bmN0aW9uKGUpe3JldHVybiB0LnNldChlKX0sITEpfSl9fSx7a2V5OiJnZXQiLHZhbHVlOmZ1bmN0aW9uKGUpe2lmKCFuLmVuYWJsZWR8fCFsLmV2ZW50KGUpKXJldHVybiBudWxsO3ZhciB0LHI9ZS50YXJnZXQsdT1lLmNoYW5nZWRUb3VjaGVzWzBdLGk9cGFyc2VGbG9hdChyLmdldEF0dHJpYnV0ZSgibWluIikpfHwwLG89cGFyc2VGbG9hdChyLmdldEF0dHJpYnV0ZSgibWF4IikpfHwxMDAsYT1wYXJzZUZsb2F0KHIuZ2V0QXR0cmlidXRlKCJzdGVwIikpfHwxLGM9ci5nZXRCb3VuZGluZ0NsaWVudFJlY3QoKSxmPTEwMC9jLndpZHRoKih0aGlzLmNvbmZpZy50aHVtYldpZHRoLzIpLzEwMDtyZXR1cm4gMD4odD0xMDAvYy53aWR0aCoodS5jbGllbnRYLWMubGVmdCkpP3Q9MDoxMDA8dCYmKHQ9MTAwKSw1MD50P3QtPSgxMDAtMip0KSpmOjUwPHQmJih0Kz0yKih0LTUwKSpmKSxpK3ModC8xMDAqKG8taSksYSl9fSx7a2V5OiJzZXQiLHZhbHVlOmZ1bmN0aW9uKGUpe24uZW5hYmxlZCYmbC5ldmVudChlKSYmIWUudGFyZ2V0LmRpc2FibGVkJiYoZS5wcmV2ZW50RGVmYXVsdCgpLGUudGFyZ2V0LnZhbHVlPXRoaXMuZ2V0KGUpLGZ1bmN0aW9uKGUsdCl7aWYoZSYmdCl7dmFyIG49bmV3IEV2ZW50KHQpO2UuZGlzcGF0Y2hFdmVudChuKX19KGUudGFyZ2V0LCJ0b3VjaGVuZCI9PT1lLnR5cGU/ImNoYW5nZSI6ImlucHV0IikpfX1dKSYmZShyLnByb3RvdHlwZSx1KSxpJiZlKHIsaSksbjt2YXIgcix1LGl9KCl9KTs8L3NjcmlwdD48L2hlYWQ+PGJvZHk+PGgyPiVTZXJ2ZXJOYW1lJTwvaDI+PGxhYmVsPjxpbnB1dCB0eXBlPSJjaGVja2JveCIgaWQ9Im15TXV0ZSIgb25jaGFuZ2U9InNldEluZm8oKSIgPiVNdXRlJTwvbGFiZWw+PGJyPjxicj48aW5wdXQgdHlwZT0icmFuZ2UiIG1pbj0iMCIgbWF4PSIxMDAiIHZhbHVlPSI1MCIgY2xhc3M9InNsaWRlciIgaWQ9Im15UmFuZ2UiIG9uY2hhbmdlPSJzZXRJbmZvKCkiPjxicj48bGFiZWwgY2xhc3M9InR4dFZvbCIgaWQ9Im15VGV4dCIgPjwvbGFiZWw+PGJyPjxicj48YnV0dG9uIG9uY2xpY2s9ImdldEluZm8oKSIgPiVSZWZyZXNoJTwvYnV0dG9uPiAgJm5ic3A7PGJ1dHRvbiBvbmNsaWNrPSJsb2NrU2NyZWVuKCkiID4lTG9ja1NjcmVlbiU8L2J1dHRvbj48YnI+PGJyPjxoMj5mb29iYXIgJVBsYXllciU8L2gyPjxidXR0b24gb25jbGljaz0iZm9vYmFyU3RhcnQoKSIgPiVTdGFydCU8L2J1dHRvbj4gICZuYnNwOzxidXR0b24gb25jbGljaz0iZm9vYmFyU3RvcCgpIiA+JVN0b3AlPC9idXR0b24+ICAmbmJzcDs8YnV0dG9uIG9uY2xpY2s9ImZvb2JhclRvZ2dsZSgpIiA+JVRvZ2dsZSU8L2J1dHRvbj4mbmJzcDs8YnV0dG9uIG9uY2xpY2s9ImZvb2JhclByZXZpb3VzKCkiID4lUHJldmlvdXMlPC9idXR0b24+ICAmbmJzcDs8YnV0dG9uIG9uY2xpY2s9ImZvb2Jhck5leHQoKSIgPiVOZXh0JTwvYnV0dG9uPjwvYm9keT48L2h0bWw+Cg=="
                    ))
                .Replace("%ServerName%", DefaultServerName)
                .Replace("%Mute%", StrMute)
                .Replace("%Refresh%", StrRefresh)
                .Replace("%LockScreen%", StrLockScreen)
                .Replace("%Vol%", StrVolume)
                .Replace("%Start%", StrStart)
                .Replace("%Stop%", StrStop)
                .Replace("%Previous%", StrPrevious)
                .Replace("%Next%", StrNext)
                .Replace("%Toggle%", StrToggle)
                .Replace("%Player%", StrPlayer);
        }

        public static bool AutoRunApp
        {
            get => _autoRunApp;
            set
            {
                if (value == _autoRunApp) return;
                _autoRunApp = value;
                SetShortcut(value);
            }
        }

 


        public static bool AutoRunServer
        {
            get => _autoRunServer;
            set
            {
                if (value == _autoRunServer) return;
                _autoRunServer = value;
                WriteConfig();
            }
        }

        public static int FoobarPort
        {
            get => (int)_port;
            set
            {
                if (value == _port) return;
                _port = (ushort)value;
                BeefwebClient.Port = value;
                WriteConfig();
            }
        }


        public static void RunApp<T>(string mutexStr) where T : Form, new()
        {
            if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                using (var mutex = new Mutex(true, mutexStr, out var createNew))
                {
                    if (createNew)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        try
                        {
                            Application.Run(new T());
                        }
#pragma warning disable CA1031 // Do not catch general exception types
                        catch (Exception e)
                        {
                            using (var sw = new StreamWriter(_errorLogPath, true))
                            {
                                sw.WriteLine("=======error==============");
                                sw.WriteLine(e.Source.ToString());
                                sw.WriteLine(e.TargetSite);
                                sw.WriteLine(e.Message);
                                sw.WriteLine(e.StackTrace);
                            }
                        }
#pragma warning restore CA1031 // Do not catch general exception types

                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                using (var me = Process.GetCurrentProcess())
                {
                    if (me.StartInfo.Verb == "runas")
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = me.ProcessName,
                            Verb = "runas"
                        });
                    }
                }
            }
        }



        private static void WriteConfig()
        {
            using (var fs = new FileStream(_savefilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                var bt = BitConverter.GetBytes(_port);
                var buf = new byte[] {
                    _autoRunServer ? (byte)1 : (byte)0,
                bt[0],bt[1]};
                fs.Write(buf, 0, 3);
            }
        }


        public static string GetLocalIP()
        {
            try
            {
                IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress item in IpEntry.AddressList)
                {
                    if (item.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return item.ToString();
                    }
                }
                return "";
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { return ""; }
#pragma warning restore CA1031 // Do not catch general exception types
        }

 


        private static readonly string _myAutoRunPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\10275915-be49-4291-86a1-c2622c7146fe.lnk";
 
       private static void SetShortcut(bool vaule)
        {
            if (File.Exists(_myAutoRunPath)) File.Delete(_myAutoRunPath);

            if (!vaule) return;         
            var link = (IShellLinkW) new ShellLink ();
            try
            {
                link.SetDescription("RemoteVolumeController");
                link.SetPath(Application.ExecutablePath);
                IPersistFile file = (IPersistFile)link;
                file.Save(_myAutoRunPath, false);
            }
            finally
            {

                Marshal.ReleaseComObject(link);
            }
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLink 
        {
        }



        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLinkW
        {
            int Reserve1();
            int Reserve2();
            int Reserve3();
            int Reserve4();

            [PreserveSig]
            int SetDescription([MarshalAs(UnmanagedType.LPWStr), In] string pszName);

            int Reserve5();
            int Reserve6();
            int Reserve7();
            int Reserve8();
            int Reserve9();
            int Reserve10();
            int Reserve11();
            int Reserve12();
            int Reserve13();
            int Reserve14();
            int Reserve15();
            int Reserve16();

            [PreserveSig]
            int SetPath([MarshalAs(UnmanagedType.LPWStr), In] string pszFile);
        }

    }
}
