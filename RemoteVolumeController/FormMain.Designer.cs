namespace RemoteVolumeController
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.CheckBoxMute = new System.Windows.Forms.CheckBox();
            this.TrackBarVol = new System.Windows.Forms.TrackBar();
            this.LabelVol = new System.Windows.Forms.Label();
            this.MyNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.MyContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemShow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.GroupBoxVol = new System.Windows.Forms.GroupBox();
            this.GroupBoxConfig = new System.Windows.Forms.GroupBox();
            this.TextBoxAddress = new System.Windows.Forms.TextBox();
            this.CheckBoxAutoRunServer = new System.Windows.Forms.CheckBox();
            this.CheckBoxStartServer = new System.Windows.Forms.CheckBox();
            this.CheckBoxAutoRunApp = new System.Windows.Forms.CheckBox();
            this.LabelBeefwebPort = new System.Windows.Forms.Label();
            this.TextBoxFoobarPort = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBarVol)).BeginInit();
            this.MyContextMenuStrip.SuspendLayout();
            this.GroupBoxVol.SuspendLayout();
            this.GroupBoxConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckBoxMute
            // 
            this.CheckBoxMute.AutoSize = true;
            this.CheckBoxMute.Location = new System.Drawing.Point(9, 20);
            this.CheckBoxMute.Name = "CheckBoxMute";
            this.CheckBoxMute.Size = new System.Drawing.Size(48, 16);
            this.CheckBoxMute.TabIndex = 0;
            this.CheckBoxMute.TabStop = false;
            this.CheckBoxMute.Text = "静音";
            this.CheckBoxMute.UseVisualStyleBackColor = true;
            this.CheckBoxMute.CheckedChanged += new System.EventHandler(this.CheckBoxMute_CheckedChanged);
            // 
            // TrackBarVol
            // 
            this.TrackBarVol.AutoSize = false;
            this.TrackBarVol.Location = new System.Drawing.Point(54, 17);
            this.TrackBarVol.Maximum = 100;
            this.TrackBarVol.Name = "TrackBarVol";
            this.TrackBarVol.Size = new System.Drawing.Size(247, 28);
            this.TrackBarVol.TabIndex = 1;
            this.TrackBarVol.TabStop = false;
            this.TrackBarVol.TickStyle = System.Windows.Forms.TickStyle.None;
            this.TrackBarVol.Scroll += new System.EventHandler(this.TrackBarVol_Scroll);
            // 
            // LabelVol
            // 
            this.LabelVol.Location = new System.Drawing.Point(307, 21);
            this.LabelVol.Name = "LabelVol";
            this.LabelVol.Size = new System.Drawing.Size(23, 15);
            this.LabelVol.TabIndex = 2;
            this.LabelVol.Text = "100";
            // 
            // MyNotifyIcon
            // 
            this.MyNotifyIcon.ContextMenuStrip = this.MyContextMenuStrip;
            this.MyNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("MyNotifyIcon.Icon")));
            this.MyNotifyIcon.Text = "远程音量控制";
            this.MyNotifyIcon.Visible = true;
            this.MyNotifyIcon.DoubleClick += new System.EventHandler(this.MyNotifyIcon_DoubleClick);
            // 
            // MyContextMenuStrip
            // 
            this.MyContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemShow,
            this.toolStripSeparator1,
            this.ToolStripMenuItemExit});
            this.MyContextMenuStrip.Name = "MyContextMenuStrip";
            this.MyContextMenuStrip.Size = new System.Drawing.Size(157, 54);
            // 
            // ToolStripMenuItemShow
            // 
            this.ToolStripMenuItemShow.Name = "ToolStripMenuItemShow";
            this.ToolStripMenuItemShow.Size = new System.Drawing.Size(156, 22);
            this.ToolStripMenuItemShow.Text = "打开主窗口(&M)";
            this.ToolStripMenuItemShow.Click += new System.EventHandler(this.ToolStripMenuItemShow_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(153, 6);
            // 
            // ToolStripMenuItemExit
            // 
            this.ToolStripMenuItemExit.Name = "ToolStripMenuItemExit";
            this.ToolStripMenuItemExit.Size = new System.Drawing.Size(156, 22);
            this.ToolStripMenuItemExit.Text = "退出(&Q)";
            this.ToolStripMenuItemExit.Click += new System.EventHandler(this.ToolStripMenuItemExit_Click);
            // 
            // GroupBoxVol
            // 
            this.GroupBoxVol.Controls.Add(this.CheckBoxMute);
            this.GroupBoxVol.Controls.Add(this.TrackBarVol);
            this.GroupBoxVol.Controls.Add(this.LabelVol);
            this.GroupBoxVol.Location = new System.Drawing.Point(12, 12);
            this.GroupBoxVol.Name = "GroupBoxVol";
            this.GroupBoxVol.Size = new System.Drawing.Size(338, 51);
            this.GroupBoxVol.TabIndex = 5;
            this.GroupBoxVol.TabStop = false;
            this.GroupBoxVol.Text = "音量";
            // 
            // GroupBoxConfig
            // 
            this.GroupBoxConfig.Controls.Add(this.TextBoxAddress);
            this.GroupBoxConfig.Controls.Add(this.CheckBoxAutoRunServer);
            this.GroupBoxConfig.Controls.Add(this.CheckBoxStartServer);
            this.GroupBoxConfig.Controls.Add(this.CheckBoxAutoRunApp);
            this.GroupBoxConfig.Location = new System.Drawing.Point(12, 71);
            this.GroupBoxConfig.Name = "GroupBoxConfig";
            this.GroupBoxConfig.Size = new System.Drawing.Size(338, 74);
            this.GroupBoxConfig.TabIndex = 6;
            this.GroupBoxConfig.TabStop = false;
            this.GroupBoxConfig.Text = "设置";
            // 
            // TextBoxAddress
            // 
            this.TextBoxAddress.Location = new System.Drawing.Point(110, 44);
            this.TextBoxAddress.Name = "TextBoxAddress";
            this.TextBoxAddress.ReadOnly = true;
            this.TextBoxAddress.Size = new System.Drawing.Size(222, 21);
            this.TextBoxAddress.TabIndex = 8;
            // 
            // CheckBoxAutoRunServer
            // 
            this.CheckBoxAutoRunServer.AutoSize = true;
            this.CheckBoxAutoRunServer.Location = new System.Drawing.Point(151, 20);
            this.CheckBoxAutoRunServer.Name = "CheckBoxAutoRunServer";
            this.CheckBoxAutoRunServer.Size = new System.Drawing.Size(132, 16);
            this.CheckBoxAutoRunServer.TabIndex = 7;
            this.CheckBoxAutoRunServer.Text = "Auto enable server";
            this.CheckBoxAutoRunServer.UseVisualStyleBackColor = true;
            this.CheckBoxAutoRunServer.Click += new System.EventHandler(this.CheckBoxAutoRunServer_Click);
            // 
            // CheckBoxStartServer
            // 
            this.CheckBoxStartServer.AutoSize = true;
            this.CheckBoxStartServer.Location = new System.Drawing.Point(9, 46);
            this.CheckBoxStartServer.Name = "CheckBoxStartServer";
            this.CheckBoxStartServer.Size = new System.Drawing.Size(102, 16);
            this.CheckBoxStartServer.TabIndex = 6;
            this.CheckBoxStartServer.Text = "Enable server";
            this.CheckBoxStartServer.UseVisualStyleBackColor = true;
            this.CheckBoxStartServer.Click += new System.EventHandler(this.CheckBoxStartServer_Click);
            // 
            // CheckBoxAutoRunApp
            // 
            this.CheckBoxAutoRunApp.AutoSize = true;
            this.CheckBoxAutoRunApp.Location = new System.Drawing.Point(9, 20);
            this.CheckBoxAutoRunApp.Name = "CheckBoxAutoRunApp";
            this.CheckBoxAutoRunApp.Size = new System.Drawing.Size(138, 16);
            this.CheckBoxAutoRunApp.TabIndex = 5;
            this.CheckBoxAutoRunApp.Text = "Auto run at startup";
            this.CheckBoxAutoRunApp.UseVisualStyleBackColor = true;
            this.CheckBoxAutoRunApp.Click += new System.EventHandler(this.CheckBoxAutoRunApp_Click);
            // 
            // LabelBeefwebPort
            // 
            this.LabelBeefwebPort.AutoSize = true;
            this.LabelBeefwebPort.Location = new System.Drawing.Point(12, 155);
            this.LabelBeefwebPort.Name = "LabelBeefwebPort";
            this.LabelBeefwebPort.Size = new System.Drawing.Size(275, 12);
            this.LabelBeefwebPort.TabIndex = 7;
            this.LabelBeefwebPort.Text = "Beefweb(foobar remote control component) Port";
            // 
            // TextBoxFoobarPort
            // 
            this.TextBoxFoobarPort.Location = new System.Drawing.Point(293, 152);
            this.TextBoxFoobarPort.MaxLength = 5;
            this.TextBoxFoobarPort.Name = "TextBoxFoobarPort";
            this.TextBoxFoobarPort.Size = new System.Drawing.Size(57, 21);
            this.TextBoxFoobarPort.TabIndex = 100;
            this.TextBoxFoobarPort.TabStop = false;
            this.TextBoxFoobarPort.TextChanged += new System.EventHandler(this.TextBoxFoobarPort_TextChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 177);
            this.Controls.Add(this.TextBoxFoobarPort);
            this.Controls.Add(this.LabelBeefwebPort);
            this.Controls.Add(this.GroupBoxConfig);
            this.Controls.Add(this.GroupBoxVol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Volume Controll";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.TrackBarVol)).EndInit();
            this.MyContextMenuStrip.ResumeLayout(false);
            this.GroupBoxVol.ResumeLayout(false);
            this.GroupBoxVol.PerformLayout();
            this.GroupBoxConfig.ResumeLayout(false);
            this.GroupBoxConfig.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox CheckBoxMute;
        private System.Windows.Forms.TrackBar TrackBarVol;
        private System.Windows.Forms.Label LabelVol;
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        private System.Windows.Forms.GroupBox GroupBoxVol;
        private System.Windows.Forms.GroupBox GroupBoxConfig;
        private System.Windows.Forms.CheckBox CheckBoxStartServer;
        private System.Windows.Forms.CheckBox CheckBoxAutoRunApp;
        private System.Windows.Forms.CheckBox CheckBoxAutoRunServer;
        private System.Windows.Forms.TextBox TextBoxAddress;
        private System.Windows.Forms.ContextMenuStrip MyContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemShow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemExit;
        private System.Windows.Forms.Label LabelBeefwebPort;
        private System.Windows.Forms.TextBox TextBoxFoobarPort;
    }
}

