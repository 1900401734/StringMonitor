namespace TCPServer
{
    partial class FormServer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toolTip1 = new ToolTip(components);
            panel1 = new Panel();
            groupBox2 = new GroupBox();
            txtLog = new TextBox();
            groupBox1 = new GroupBox();
            lblInfoIcon_AutoLaunch = new Label();
            lblInfoIcon_SilentStart = new Label();
            chkSilentStart = new CheckBox();
            chkAutoLaunch = new CheckBox();
            groupBox3 = new GroupBox();
            lblDataPath = new Label();
            label1 = new Label();
            btnOpenFile = new Button();
            btnChangePath = new Button();
            btnStop = new Button();
            btnSave = new Button();
            btnStart = new Button();
            txtPort = new TextBox();
            txtIP = new TextBox();
            status = new Label();
            lblStatus = new Label();
            lblIP = new Label();
            lblPort = new Label();
            lblEncodingFormat = new Label();
            panel1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(groupBox2);
            panel1.Controls.Add(groupBox1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(880, 432);
            panel1.TabIndex = 1;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(lblEncodingFormat);
            groupBox2.Controls.Add(txtLog);
            groupBox2.Location = new Point(436, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(444, 432);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "交互日志";
            // 
            // txtLog
            // 
            txtLog.Dock = DockStyle.Fill;
            txtLog.Location = new Point(3, 26);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new Size(438, 403);
            txtLog.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(lblInfoIcon_AutoLaunch);
            groupBox1.Controls.Add(lblInfoIcon_SilentStart);
            groupBox1.Controls.Add(chkSilentStart);
            groupBox1.Controls.Add(chkAutoLaunch);
            groupBox1.Controls.Add(groupBox3);
            groupBox1.Controls.Add(btnStop);
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(btnStart);
            groupBox1.Controls.Add(txtPort);
            groupBox1.Controls.Add(txtIP);
            groupBox1.Controls.Add(status);
            groupBox1.Controls.Add(lblStatus);
            groupBox1.Controls.Add(lblIP);
            groupBox1.Controls.Add(lblPort);
            groupBox1.Font = new Font("Segoe UI", 8.25F);
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(429, 432);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "基本配置";
            // 
            // lblInfoIcon_AutoLaunch
            // 
            lblInfoIcon_AutoLaunch.AutoSize = true;
            lblInfoIcon_AutoLaunch.Cursor = Cursors.Help;
            lblInfoIcon_AutoLaunch.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            lblInfoIcon_AutoLaunch.ForeColor = Color.FromArgb(70, 130, 180);
            lblInfoIcon_AutoLaunch.Location = new Point(124, 334);
            lblInfoIcon_AutoLaunch.Name = "lblInfoIcon_AutoLaunch";
            lblInfoIcon_AutoLaunch.Size = new Size(30, 28);
            lblInfoIcon_AutoLaunch.TabIndex = 3;
            lblInfoIcon_AutoLaunch.Text = "ⓘ";
            lblInfoIcon_AutoLaunch.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblInfoIcon_SilentStart
            // 
            lblInfoIcon_SilentStart.AutoSize = true;
            lblInfoIcon_SilentStart.Cursor = Cursors.Help;
            lblInfoIcon_SilentStart.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            lblInfoIcon_SilentStart.ForeColor = Color.FromArgb(70, 130, 180);
            lblInfoIcon_SilentStart.Location = new Point(124, 375);
            lblInfoIcon_SilentStart.Name = "lblInfoIcon_SilentStart";
            lblInfoIcon_SilentStart.Size = new Size(30, 28);
            lblInfoIcon_SilentStart.TabIndex = 3;
            lblInfoIcon_SilentStart.Text = "ⓘ";
            lblInfoIcon_SilentStart.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // chkSilentStart
            // 
            chkSilentStart.AutoSize = true;
            chkSilentStart.Font = new Font("Microsoft YaHei UI", 10F);
            chkSilentStart.Location = new Point(12, 375);
            chkSilentStart.Name = "chkSilentStart";
            chkSilentStart.Size = new Size(118, 31);
            chkSilentStart.TabIndex = 2;
            chkSilentStart.Text = "静默启动";
            chkSilentStart.UseVisualStyleBackColor = true;
            // 
            // chkAutoLaunch
            // 
            chkAutoLaunch.AutoSize = true;
            chkAutoLaunch.Font = new Font("Microsoft YaHei UI", 10F);
            chkAutoLaunch.Location = new Point(13, 332);
            chkAutoLaunch.Name = "chkAutoLaunch";
            chkAutoLaunch.Size = new Size(118, 31);
            chkAutoLaunch.TabIndex = 2;
            chkAutoLaunch.Text = "开机自启";
            chkAutoLaunch.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(lblDataPath);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(btnOpenFile);
            groupBox3.Controls.Add(btnChangePath);
            groupBox3.Font = new Font("Microsoft YaHei UI", 10F);
            groupBox3.Location = new Point(15, 174);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(395, 144);
            groupBox3.TabIndex = 1;
            groupBox3.TabStop = false;
            groupBox3.Text = "存放路径";
            // 
            // lblDataPath
            // 
            lblDataPath.AutoSize = true;
            lblDataPath.Location = new Point(122, 38);
            lblDataPath.Name = "lblDataPath";
            lblDataPath.Size = new Size(85, 27);
            lblDataPath.TabIndex = 0;
            lblDataPath.Text = "D:\\Logs";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 38);
            label1.Name = "label1";
            label1.Size = new Size(112, 27);
            label1.TabIndex = 0;
            label1.Text = "当前路径：";
            // 
            // btnOpenFile
            // 
            btnOpenFile.AutoSize = true;
            btnOpenFile.Location = new Point(217, 85);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(150, 37);
            btnOpenFile.TabIndex = 0;
            btnOpenFile.Text = "查看文件位置";
            btnOpenFile.UseVisualStyleBackColor = true;
            btnOpenFile.Click += btnOpenFile_Click;
            // 
            // btnChangePath
            // 
            btnChangePath.AutoSize = true;
            btnChangePath.Location = new Point(24, 85);
            btnChangePath.Name = "btnChangePath";
            btnChangePath.Size = new Size(150, 37);
            btnChangePath.TabIndex = 0;
            btnChangePath.Text = "变更存放路径";
            btnChangePath.UseVisualStyleBackColor = true;
            btnChangePath.Click += btnChangePath_Click;
            // 
            // btnStop
            // 
            btnStop.AutoSize = true;
            btnStop.Font = new Font("Microsoft YaHei UI", 10F);
            btnStop.Location = new Point(309, 124);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(101, 37);
            btnStop.TabIndex = 0;
            btnStop.Text = "停止";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnSave
            // 
            btnSave.AutoSize = true;
            btnSave.Font = new Font("Microsoft YaHei UI", 10F);
            btnSave.Location = new Point(309, 28);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(101, 37);
            btnSave.TabIndex = 0;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnStart
            // 
            btnStart.AutoSize = true;
            btnStart.Font = new Font("Microsoft YaHei UI", 10F);
            btnStart.Location = new Point(309, 78);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(101, 37);
            btnStart.TabIndex = 0;
            btnStart.Text = "启动";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // txtPort
            // 
            txtPort.Font = new Font("Microsoft YaHei UI", 10F);
            txtPort.Location = new Point(104, 126);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(165, 33);
            txtPort.TabIndex = 1;
            // 
            // txtIP
            // 
            txtIP.Font = new Font("Microsoft YaHei UI", 10F);
            txtIP.Location = new Point(104, 80);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(165, 33);
            txtIP.TabIndex = 0;
            // 
            // status
            // 
            status.AutoSize = true;
            status.Font = new Font("Microsoft YaHei UI", 10F);
            status.Location = new Point(15, 37);
            status.Name = "status";
            status.Size = new Size(90, 27);
            status.TabIndex = 0;
            status.Text = "Status：";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Microsoft YaHei UI", 10F);
            lblStatus.Location = new Point(104, 37);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(138, 27);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "disconnected";
            // 
            // lblIP
            // 
            lblIP.AutoSize = true;
            lblIP.Font = new Font("Microsoft YaHei UI", 10F);
            lblIP.Location = new Point(52, 80);
            lblIP.Name = "lblIP";
            lblIP.Size = new Size(50, 27);
            lblIP.TabIndex = 0;
            lblIP.Text = "IP：";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Font = new Font("Microsoft YaHei UI", 10F);
            lblPort.Location = new Point(33, 126);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(72, 27);
            lblPort.TabIndex = 0;
            lblPort.Text = "Port：";
            // 
            // lblEncodingFormat
            // 
            lblEncodingFormat.AutoSize = true;
            lblEncodingFormat.Font = new Font("Georgia", 7F);
            lblEncodingFormat.Location = new Point(112, 6);
            lblEncodingFormat.Name = "lblEncodingFormat";
            lblEncodingFormat.Size = new Size(148, 17);
            lblEncodingFormat.TabIndex = 1;
            lblEncodingFormat.Text = "当前字符串编码格式：";
            // 
            // FormServer
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(880, 432);
            Controls.Add(panel1);
            Name = "FormServer";
            Text = "Form1";
            FormClosing += FormServer_FormClosing;
            panel1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private ToolTip toolTip1;
        private Panel panel1;
        private GroupBox groupBox2;
        private TextBox txtLog;
        private GroupBox groupBox1;
        private Label lblInfoIcon_SilentStart;
        private CheckBox chkSilentStart;
        private CheckBox chkAutoLaunch;
        private GroupBox groupBox3;
        private Label lblDataPath;
        private Label label1;
        private Button btnOpenFile;
        private Button btnChangePath;
        private Button btnStop;
        private Button btnSave;
        private Button btnStart;
        private TextBox txtPort;
        private TextBox txtIP;
        private Label status;
        private Label lblStatus;
        private Label lblIP;
        private Label lblPort;
        private Label lblInfoIcon_AutoLaunch;
        private Label lblEncodingFormat;
    }
}
