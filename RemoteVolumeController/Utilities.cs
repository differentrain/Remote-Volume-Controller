using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteVolumeController
{
    internal static class Utilities
    {
        private static readonly string _savefilePath = Application.StartupPath + @"\RemoteVolumeController.config";

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


        public static readonly string DefaultServerName;
        public static readonly string HtmpTempl;

        private static bool _autoRunApp = false;
        private static bool _autoRunServer = false;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static Utilities()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            if (File.Exists(_savefilePath))
            {
                using (var fr = new FileStream(_savefilePath, FileMode.Open, FileAccess.Read))
                {
                    if (fr.Length != 2)
                    {
                        fr.Dispose();
                        File.Delete(_savefilePath);
                        WriteConfig();
                        return;
                    }
                    _autoRunApp = fr.ReadByte() == 1;
                    _autoRunServer = fr.ReadByte() == 1;
                }
            }
            else
            {
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
            }

            DefaultServerName = Dns.GetHostName() + $" {StrVolume}";
            HtmpTempl = Encoding.UTF8.GetString(
                Convert.FromBase64String(
                    "PCFET0NUWVBFIGh0bWw+PGh0bWw+PGhlYWQ+PG1ldGEgbmFtZT0idmlld3BvcnQiIGNvbnRlbnQ9IndpZHRoPWRldmljZS13aWR0aCwgaW5pdGlhbC1zY2FsZT0xIj48bWV0YSBjaGFyc2V0PSJVVEYtOCI+PHRpdGxlPiVTZXJ2ZXJOYW1lJTwvdGl0bGU+PHN0eWxlPi5jb250YWluZXJ7ZGlzcGxheTpibG9jaztwb3NpdGlvbjpyZWxhdGl2ZTtwYWRkaW5nLWxlZnQ6MzVweDttYXJnaW4tYm90dG9tOjEycHg7Y3Vyc29yOnBvaW50ZXI7Zm9udC1zaXplOjIwcHg7LXdlYmtpdC11c2VyLXNlbGVjdDpub25lOy1tb3otdXNlci1zZWxlY3Q6bm9uZTstbXMtdXNlci1zZWxlY3Q6bm9uZTt1c2VyLXNlbGVjdDpub25lfS5jb250YWluZXIgaW5wdXR7cG9zaXRpb246YWJzb2x1dGU7b3BhY2l0eTowO2N1cnNvcjpwb2ludGVyO2hlaWdodDowO3dpZHRoOjB9LmNoZWNrbWFya3twb3NpdGlvbjphYnNvbHV0ZTt0b3A6MDtsZWZ0OjA7aGVpZ2h0OjI1cHg7d2lkdGg6MjVweDtiYWNrZ3JvdW5kLWNvbG9yOiNlZWV9LmNvbnRhaW5lcjpob3ZlciBpbnB1dCB+IC5jaGVja21hcmt7YmFja2dyb3VuZC1jb2xvcjojY2NjfS5jb250YWluZXIgaW5wdXQ6Y2hlY2tlZCB+IC5jaGVja21hcmt7YmFja2dyb3VuZC1jb2xvcjojMjE5NmYzfS5jaGVja21hcms6YWZ0ZXJ7Y29udGVudDoiIjtwb3NpdGlvbjphYnNvbHV0ZTtkaXNwbGF5Om5vbmV9LmNvbnRhaW5lciBpbnB1dDpjaGVja2VkIH4gLmNoZWNrbWFyazphZnRlcntkaXNwbGF5OmJsb2NrfS5jb250YWluZXIgLmNoZWNrbWFyazphZnRlcntsZWZ0OjlweDt0b3A6NXB4O3dpZHRoOjVweDtoZWlnaHQ6MTBweDtib3JkZXI6c29saWQgd2hpdGU7Ym9yZGVyLXdpZHRoOjAgM3B4IDNweCAwOy13ZWJraXQtdHJhbnNmb3JtOnJvdGF0ZSg0NWRlZyk7LW1zLXRyYW5zZm9ybTpyb3RhdGUoNDVkZWcpO3RyYW5zZm9ybTpyb3RhdGUoNDVkZWcpfS5zbGlkZXJ7LXdlYmtpdC1hcHBlYXJhbmNlOm5vbmU7d2lkdGg6OTglO2hlaWdodDoyNXB4O2JhY2tncm91bmQ6I2QzZDNkMztvdXRsaW5lOjA7b3BhY2l0eTouNzstd2Via2l0LXRyYW5zaXRpb246LjJzO3RyYW5zaXRpb246b3BhY2l0eSAuMnN9LnNsaWRlcjpob3ZlcntvcGFjaXR5OjF9LnNsaWRlcjo6LXdlYmtpdC1zbGlkZXItdGh1bWJ7LXdlYmtpdC1hcHBlYXJhbmNlOm5vbmU7YXBwZWFyYW5jZTpub25lO3dpZHRoOjI1cHg7aGVpZ2h0OjI1cHg7YmFja2dyb3VuZDojNGNhZjUwO2N1cnNvcjpwb2ludGVyfS5zbGlkZXI6Oi1tb3otcmFuZ2UtdGh1bWJ7d2lkdGg6MjVweDtoZWlnaHQ6MjVweDtiYWNrZ3JvdW5kOiM0Y2FmNTA7Y3Vyc29yOnBvaW50ZXJ9LnR4dFZvbHtmb250LXNpemU6MjBweH08L3N0eWxlPjxzY3JpcHQgdHlwZT0idGV4dC9qYXZhc2NyaXB0Ij53aW5kb3cub25sb2FkPWZ1bmN0aW9uKCl7Z2V0SW5mbygpO3ZvbCA9IGRvY3VtZW50LmdldEVsZW1lbnRCeUlkKCJteVJhbmdlIik7Y29uc3QgcmFuZ2VzID0gUmFuZ2VUb3VjaC5zZXR1cCh2b2wpO307ZnVuY3Rpb24gZ2V0SW5mbygpe3ZvbD1kb2N1bWVudC5nZXRFbGVtZW50QnlJZCgibXlSYW5nZSIpO3R4dD1kb2N1bWVudC5nZXRFbGVtZW50QnlJZCgibXlUZXh0Iik7bXV0ZT1kb2N1bWVudC5nZXRFbGVtZW50QnlJZCgibXlNdXRlIik7dmFyIHhtbGh0dHA9bmV3IFhNTEh0dHBSZXF1ZXN0KCk7eG1saHR0cC5vbnJlYWR5c3RhdGVjaGFuZ2U9ZnVuY3Rpb24oKXtpZih4bWxodHRwLnJlYWR5U3RhdGU9PTQmJnhtbGh0dHAuc3RhdHVzPT0yMDApe3ZhciBqc29uPUpTT04ucGFyc2UoeG1saHR0cC5yZXNwb25zZVRleHQpO211dGUuY2hlY2tlZD1qc29uLm11dGU7dm9sLnZhbHVlPWpzb24udm9sO3R4dC5pbm5lckhUTUw9IiVWb2wl77yaIit2b2wudmFsdWV9fTt4bWxodHRwLm9wZW4oIkdFVCIsIi92b2x1bWUvYXBpL2dldGluZm8iLHRydWUpO3htbGh0dHAuc2VuZCgpfWZ1bmN0aW9uIHNldEluZm8oKXt2YXIgeG1saHR0cD1uZXcgWE1MSHR0cFJlcXVlc3QoKTt2b2w9ZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoIm15UmFuZ2UiKTt0eHQ9ZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoIm15VGV4dCIpO3R4dC5pbm5lckhUTUw9IiVWb2wl77yaIit2b2wudmFsdWU7bXV0ZT1kb2N1bWVudC5nZXRFbGVtZW50QnlJZCgibXlNdXRlIik7eG1saHR0cC5vcGVuKCJHRVQiLCIvdm9sdW1lL2FwaS9zZXRpbmZvPyIrIm11dGU9IittdXRlLmNoZWNrZWQrIiZ2b2w9Iit2b2wudmFsdWUsdHJ1ZSk7eG1saHR0cC5zZW5kKCl9O2Z1bmN0aW9uIGxvY2tTY3JlZW4oKXt2YXIgeG1saHR0cD1uZXcgWE1MSHR0cFJlcXVlc3QoKTt4bWxodHRwLm9wZW4oIkdFVCIsIi92b2x1bWUvYXBpL2xvY2tzY3JlZW4iLHRydWUpO3htbGh0dHAuc2VuZCgpfTshZnVuY3Rpb24oZSx0KXsib2JqZWN0Ij09dHlwZW9mIGV4cG9ydHMmJiJ1bmRlZmluZWQiIT10eXBlb2YgbW9kdWxlP21vZHVsZS5leHBvcnRzPXQoKToiZnVuY3Rpb24iPT10eXBlb2YgZGVmaW5lJiZkZWZpbmUuYW1kP2RlZmluZSgiUmFuZ2VUb3VjaCIsdCk6ZS5SYW5nZVRvdWNoPXQoKX0odGhpcyxmdW5jdGlvbigpeyJ1c2Ugc3RyaWN0IjtmdW5jdGlvbiBlKGUsdCl7Zm9yKHZhciBuPTA7bjx0Lmxlbmd0aDtuKyspe3ZhciByPXRbbl07ci5lbnVtZXJhYmxlPXIuZW51bWVyYWJsZXx8ITEsci5jb25maWd1cmFibGU9ITAsInZhbHVlImluIHImJihyLndyaXRhYmxlPSEwKSxPYmplY3QuZGVmaW5lUHJvcGVydHkoZSxyLmtleSxyKX19dmFyIHQ9e2FkZENTUzohMCx0aHVtYldpZHRoOjE1LHdhdGNoOiEwfTt2YXIgbj1mdW5jdGlvbihlKXtyZXR1cm4gbnVsbCE9ZT9lLmNvbnN0cnVjdG9yOm51bGx9LHI9ZnVuY3Rpb24oZSx0KXtyZXR1cm4hIShlJiZ0JiZlIGluc3RhbmNlb2YgdCl9LHU9ZnVuY3Rpb24oZSl7cmV0dXJuIG51bGw9PWV9LGk9ZnVuY3Rpb24oZSl7cmV0dXJuIG4oZSk9PT1PYmplY3R9LG89ZnVuY3Rpb24oZSl7cmV0dXJuIG4oZSk9PT1TdHJpbmd9LGE9ZnVuY3Rpb24oZSl7cmV0dXJuIEFycmF5LmlzQXJyYXkoZSl9LGM9ZnVuY3Rpb24oZSl7cmV0dXJuIHIoZSxOb2RlTGlzdCl9LGw9e251bGxPclVuZGVmaW5lZDp1LG9iamVjdDppLG51bWJlcjpmdW5jdGlvbihlKXtyZXR1cm4gbihlKT09PU51bWJlciYmIU51bWJlci5pc05hTihlKX0sc3RyaW5nOm8sYm9vbGVhbjpmdW5jdGlvbihlKXtyZXR1cm4gbihlKT09PUJvb2xlYW59LGZ1bmN0aW9uOmZ1bmN0aW9uKGUpe3JldHVybiBuKGUpPT09RnVuY3Rpb259LGFycmF5OmEsbm9kZUxpc3Q6YyxlbGVtZW50OmZ1bmN0aW9uKGUpe3JldHVybiByKGUsRWxlbWVudCl9LGV2ZW50OmZ1bmN0aW9uKGUpe3JldHVybiByKGUsRXZlbnQpfSxlbXB0eTpmdW5jdGlvbihlKXtyZXR1cm4gdShlKXx8KG8oZSl8fGEoZSl8fGMoZSkpJiYhZS5sZW5ndGh8fGkoZSkmJiFPYmplY3Qua2V5cyhlKS5sZW5ndGh9fTtmdW5jdGlvbiBzKGUsdCl7aWYoMT50KXt2YXIgbj1mdW5jdGlvbihlKXt2YXIgdD0iIi5jb25jYXQoZSkubWF0Y2goLyg/OlwuKFxkKykpPyg/OltlRV0oWystXT9cZCspKT8kLyk7cmV0dXJuIHQ/TWF0aC5tYXgoMCwodFsxXT90WzFdLmxlbmd0aDowKS0odFsyXT8rdFsyXTowKSk6MH0odCk7cmV0dXJuIHBhcnNlRmxvYXQoZS50b0ZpeGVkKG4pKX1yZXR1cm4gTWF0aC5yb3VuZChlL3QpKnR9cmV0dXJuIGZ1bmN0aW9uKCl7ZnVuY3Rpb24gbihlLHIpeyhmdW5jdGlvbihlLHQpe2lmKCEoZSBpbnN0YW5jZW9mIHQpKXRocm93IG5ldyBUeXBlRXJyb3IoIkNhbm5vdCBjYWxsIGEgY2xhc3MgYXMgYSBmdW5jdGlvbiIpfSkodGhpcyxuKSxsLmVsZW1lbnQoZSk/dGhpcy5lbGVtZW50PWU6bC5zdHJpbmcoZSkmJih0aGlzLmVsZW1lbnQ9ZG9jdW1lbnQucXVlcnlTZWxlY3RvcihlKSksbC5lbGVtZW50KHRoaXMuZWxlbWVudCkmJmwuZW1wdHkodGhpcy5lbGVtZW50LnJhbmdlVG91Y2gpJiYodGhpcy5jb25maWc9T2JqZWN0LmFzc2lnbih7fSx0LHIpLHRoaXMuaW5pdCgpKX1yZXR1cm4gcj1uLGk9W3trZXk6InNldHVwIix2YWx1ZTpmdW5jdGlvbihlKXt2YXIgcj0xPGFyZ3VtZW50cy5sZW5ndGgmJnZvaWQgMCE9PWFyZ3VtZW50c1sxXT9hcmd1bWVudHNbMV06e30sdT1udWxsO2lmKGwuZW1wdHkoZSl8fGwuc3RyaW5nKGUpP3U9QXJyYXkuZnJvbShkb2N1bWVudC5xdWVyeVNlbGVjdG9yQWxsKGwuc3RyaW5nKGUpP2U6J2lucHV0W3R5cGU9InJhbmdlIl0nKSk6bC5lbGVtZW50KGUpP3U9W2VdOmwubm9kZUxpc3QoZSk/dT1BcnJheS5mcm9tKGUpOmwuYXJyYXkoZSkmJih1PWUuZmlsdGVyKGwuZWxlbWVudCkpLGwuZW1wdHkodSkpcmV0dXJuIG51bGw7dmFyIGk9T2JqZWN0LmFzc2lnbih7fSx0LHIpO2wuc3RyaW5nKGUpJiZpLndhdGNoJiZuZXcgTXV0YXRpb25PYnNlcnZlcihmdW5jdGlvbih0KXtBcnJheS5mcm9tKHQpLmZvckVhY2goZnVuY3Rpb24odCl7QXJyYXkuZnJvbSh0LmFkZGVkTm9kZXMpLmZvckVhY2goZnVuY3Rpb24odCl7bC5lbGVtZW50KHQpJiZmdW5jdGlvbihlLHQpe3JldHVybiBmdW5jdGlvbigpe3JldHVybiBBcnJheS5mcm9tKGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwodCkpLmluY2x1ZGVzKHRoaXMpfS5jYWxsKGUsdCl9KHQsZSkmJm5ldyBuKHQsaSl9KX0pfSkub2JzZXJ2ZShkb2N1bWVudC5ib2R5LHtjaGlsZExpc3Q6ITAsc3VidHJlZTohMH0pO3JldHVybiB1Lm1hcChmdW5jdGlvbihlKXtyZXR1cm4gbmV3IG4oZSxyKX0pfX0se2tleToiZW5hYmxlZCIsZ2V0OmZ1bmN0aW9uKCl7cmV0dXJuIm9udG91Y2hzdGFydCJpbiBkb2N1bWVudC5kb2N1bWVudEVsZW1lbnR9fV0sKHU9W3trZXk6ImluaXQiLHZhbHVlOmZ1bmN0aW9uKCl7bi5lbmFibGVkJiYodGhpcy5jb25maWcuYWRkQ1NTJiYodGhpcy5lbGVtZW50LnN0eWxlLnVzZXJTZWxlY3Q9Im5vbmUiLHRoaXMuZWxlbWVudC5zdHlsZS53ZWJLaXRVc2VyU2VsZWN0PSJub25lIix0aGlzLmVsZW1lbnQuc3R5bGUudG91Y2hBY3Rpb249Im1hbmlwdWxhdGlvbiIpLHRoaXMubGlzdGVuZXJzKCEwKSx0aGlzLmVsZW1lbnQucmFuZ2VUb3VjaD10aGlzKX19LHtrZXk6ImRlc3Ryb3kiLHZhbHVlOmZ1bmN0aW9uKCl7bi5lbmFibGVkJiYodGhpcy5saXN0ZW5lcnMoITEpLHRoaXMuZWxlbWVudC5yYW5nZVRvdWNoPW51bGwpfX0se2tleToibGlzdGVuZXJzIix2YWx1ZTpmdW5jdGlvbihlKXt2YXIgdD10aGlzLG49ZT8iYWRkRXZlbnRMaXN0ZW5lciI6InJlbW92ZUV2ZW50TGlzdGVuZXIiO1sidG91Y2hzdGFydCIsInRvdWNobW92ZSIsInRvdWNoZW5kIl0uZm9yRWFjaChmdW5jdGlvbihlKXt0LmVsZW1lbnRbbl0oZSxmdW5jdGlvbihlKXtyZXR1cm4gdC5zZXQoZSl9LCExKX0pfX0se2tleToiZ2V0Iix2YWx1ZTpmdW5jdGlvbihlKXtpZighbi5lbmFibGVkfHwhbC5ldmVudChlKSlyZXR1cm4gbnVsbDt2YXIgdCxyPWUudGFyZ2V0LHU9ZS5jaGFuZ2VkVG91Y2hlc1swXSxpPXBhcnNlRmxvYXQoci5nZXRBdHRyaWJ1dGUoIm1pbiIpKXx8MCxvPXBhcnNlRmxvYXQoci5nZXRBdHRyaWJ1dGUoIm1heCIpKXx8MTAwLGE9cGFyc2VGbG9hdChyLmdldEF0dHJpYnV0ZSgic3RlcCIpKXx8MSxjPXIuZ2V0Qm91bmRpbmdDbGllbnRSZWN0KCksZj0xMDAvYy53aWR0aCoodGhpcy5jb25maWcudGh1bWJXaWR0aC8yKS8xMDA7cmV0dXJuIDA+KHQ9MTAwL2Mud2lkdGgqKHUuY2xpZW50WC1jLmxlZnQpKT90PTA6MTAwPHQmJih0PTEwMCksNTA+dD90LT0oMTAwLTIqdCkqZjo1MDx0JiYodCs9MioodC01MCkqZiksaStzKHQvMTAwKihvLWkpLGEpfX0se2tleToic2V0Iix2YWx1ZTpmdW5jdGlvbihlKXtuLmVuYWJsZWQmJmwuZXZlbnQoZSkmJiFlLnRhcmdldC5kaXNhYmxlZCYmKGUucHJldmVudERlZmF1bHQoKSxlLnRhcmdldC52YWx1ZT10aGlzLmdldChlKSxmdW5jdGlvbihlLHQpe2lmKGUmJnQpe3ZhciBuPW5ldyBFdmVudCh0KTtlLmRpc3BhdGNoRXZlbnQobil9fShlLnRhcmdldCwidG91Y2hlbmQiPT09ZS50eXBlPyJjaGFuZ2UiOiJpbnB1dCIpKX19XSkmJmUoci5wcm90b3R5cGUsdSksaSYmZShyLGkpLG47dmFyIHIsdSxpfSgpfSk7PC9zY3JpcHQ+PC9oZWFkPjxib2R5PjxoMj4lU2VydmVyTmFtZSU8L2gyPjxicj48bGFiZWwgY2xhc3M9ImNvbnRhaW5lciI+JU11dGUlPGlucHV0IHR5cGU9ImNoZWNrYm94IiBpZD0ibXlNdXRlIiBvbmNoYW5nZT0ic2V0SW5mbygpIiA+PHNwYW4gY2xhc3M9ImNoZWNrbWFyayI+PC9zcGFuPjwvbGFiZWw+PGJyPjxpbnB1dCB0eXBlPSJyYW5nZSIgbWluPSIwIiBtYXg9IjEwMCIgdmFsdWU9IjUwIiBjbGFzcz0ic2xpZGVyIiBpZD0ibXlSYW5nZSIgb25jaGFuZ2U9InNldEluZm8oKSI+PHAgY2xhc3M9InR4dFZvbCIgaWQ9Im15VGV4dCIgPjwvcD48YnV0dG9uIGNsYXNzPSJidG4iIG9uY2xpY2s9ImdldEluZm8oKSIgPiVSZWZyZXNoJTwvYnV0dG9uPjxicj48YnI+PGJyPjxidXR0b24gY2xhc3M9ImJ0biIgb25jbGljaz0ibG9ja1NjcmVlbigpIiA+JUxvY2tTY3JlZW4lPC9idXR0b24+PC9ib2R5PjwvaHRtbD4="
                    ))
                .Replace("%ServerName%", DefaultServerName)
                .Replace("%Mute%",StrMute)
                .Replace("%Refresh%",StrRefresh)
                .Replace("%LockScreen%", StrLockScreen)
                .Replace("%Vol%", StrVolume);
        }

        public static bool AutoRunApp
        {
            get => _autoRunApp;
            set
            {
                if (value == _autoRunApp) return;

                var cu = Registry.CurrentUser;
                using (var au = cu.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (value)
                    {
                        au.SetValue("RemoteVolumeController", Application.ExecutablePath);
                    }
                    else
                    {
                        au.DeleteValue("RemoteVolumeController", false);
                    }
                }
                cu.Close();

                _autoRunApp = value;
                WriteConfig();
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
                        Application.Run(new T());
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
                fs.WriteByte(_autoRunApp ? (byte)1 : (byte)0);
                fs.WriteByte(_autoRunServer ? (byte)1 : (byte)0);
            }
        }
    }
}
