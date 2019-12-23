using System;
using System.Windows.Forms;
using RemoteVolumeController.RemoteVolumeControllerService;
using RemoteVolumeController.SystemInfomation;

namespace RemoteVolumeController
{
    public partial class FormMain : Form
    {
        private readonly SystemVolume SysVol = new SystemVolume();
        private readonly HttpVolumeControllerServer Server;
        private readonly Action SetValue;

        private bool _isSet;

        public FormMain()
        {
            InitializeComponent();

            this.Text = Utilities.StrTitle;
            GroupBoxConfig.Text = Utilities.StrConfig;
            GroupBoxVol.Text = Utilities.StrVolume;
            CheckBoxMute.Text = Utilities.StrMute;
            CheckBoxAutoRunApp.Text = Utilities.StrAutoRunApp;
            CheckBoxAutoRunServer.Text = Utilities.StrAutoRunServer;
            CheckBoxStartServer.Text = Utilities.StrStartServer;
            MyNotifyIcon.Text = Utilities.StrTitle;
            ToolStripMenuItemShow.Text = Utilities.StrOpenMainWindow;
            ToolStripMenuItemExit.Text = Utilities.StrExit;
            LabelBeefwebPort.Text = Utilities.StrFoobarPort;

            SysVol.VolumeChanged += SysVol_VolumeChanged;
            SetValue = new Action(() =>
            {
                _isSet = true;
                var value = Math.Ceiling(SysVol.MasterVolume * 100);
                LabelVol.Text = value.ToString();
                TrackBarVol.Value = (int)value;
                CheckBoxMute.Checked = SysVol.Mute;
                _isSet = false;
            });
            SetValue();
            Server = HttpVolumeControllerServer.GetOrCreateServer(SysVol);

            CheckBoxAutoRunApp.Checked = Utilities.AutoRunApp;
            CheckBoxAutoRunServer.Checked = Utilities.AutoRunServer;

            if (Utilities.AutoRunServer)
            {
                Server.Start();
                CheckBoxStartServer.Checked = true;
                TextBoxAddress.Text = Server.ServerAddress;
            }

            _textboxReset = true;
            TextBoxFoobarPort.Text = Utilities.FoobarPort.ToString();
        }

        private void SysVol_VolumeChanged(object sender, bool e)
        {
            // if (!e) return;
            Invoke(SetValue);
        }




        private void TrackBarVol_Scroll(object sender, EventArgs e)
        {
            if (_isSet) return;
            SysVol.MasterVolume = TrackBarVol.Value / 100f;
            LabelVol.Text = TrackBarVol.Value.ToString();
        }

        private void CheckBoxMute_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSet) return;
            SysVol.Mute = CheckBoxMute.Checked;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
            {
                SysVol.Dispose();
                Server.Dispose();
            }
            else
            {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e) => Application.Exit();
        private void ToolStripMenuItemShow_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }
        private void MyNotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowWindow();
        }


        private void ShowWindow()
        {
            this.Visible = true;
            ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void CheckBoxAutoRunApp_Click(object sender, EventArgs e)
        {
            Utilities.AutoRunApp = CheckBoxAutoRunApp.Checked;
        }

        private void CheckBoxAutoRunServer_Click(object sender, EventArgs e)
        {
            Utilities.AutoRunServer = CheckBoxAutoRunServer.Checked;
        }

        private void CheckBoxStartServer_Click(object sender, EventArgs e)
        {
            if (CheckBoxStartServer.Checked)
            {
                Server.Start();
                TextBoxAddress.Text = Server.ServerAddress;
            }
            else
            {
                Server.Stop();
                TextBoxAddress.Text = string.Empty;
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.
            //Opacity = 0;

            base.OnLoad(e);
        }

        private bool _textboxReset = false;
        private void TextBoxFoobarPort_TextChanged(object sender, EventArgs e)
        {
            if (_textboxReset == true)
            {
                _textboxReset = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxFoobarPort.Text) ||
                !ushort.TryParse(TextBoxFoobarPort.Text, out var port) ||
                port < 1024)
            {
                _textboxReset = true;
                TextBoxFoobarPort.Text = Utilities.FoobarPort.ToString();
                return;
            }
            Utilities.FoobarPort = port;
        }
    }
}
