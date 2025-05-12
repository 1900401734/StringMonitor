namespace TCPClient
{
    partial class FormClient
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
            btnDisconnect = new Button();
            btnSave = new Button();
            btnConnect = new Button();
            txtServerPort = new TextBox();
            txtServerIP = new TextBox();
            lblStatus = new Label();
            lblDataPath = new Label();
            label1 = new Label();
            txtLog = new TextBox();
            btnOpenFile = new Button();
            btnChangePath = new Button();
            groupBox2 = new GroupBox();
            panel1 = new Panel();
            groupBox1 = new GroupBox();
            groupBox3 = new GroupBox();
            btnSend = new Button();
            txtMessage = new TextBox();
            lblIP = new Label();
            label2 = new Label();
            lblPort = new Label();
            groupBox2.SuspendLayout();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // btnDisconnect
            // 
            btnDisconnect.AutoSize = true;
            btnDisconnect.Location = new Point(309, 124);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(101, 34);
            btnDisconnect.TabIndex = 0;
            btnDisconnect.Text = "断开";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // btnSave
            // 
            btnSave.AutoSize = true;
            btnSave.Location = new Point(309, 28);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(101, 34);
            btnSave.TabIndex = 0;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnConnect
            // 
            btnConnect.AutoSize = true;
            btnConnect.Location = new Point(309, 78);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(101, 34);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "连接";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // txtServerPort
            // 
            txtServerPort.Location = new Point(98, 128);
            txtServerPort.Name = "txtServerPort";
            txtServerPort.Size = new Size(178, 30);
            txtServerPort.TabIndex = 1;
            // 
            // txtServerIP
            // 
            txtServerIP.Location = new Point(98, 82);
            txtServerIP.Name = "txtServerIP";
            txtServerIP.Size = new Size(178, 30);
            txtServerIP.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(33, 37);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(63, 24);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Status";
            // 
            // lblDataPath
            // 
            lblDataPath.AutoSize = true;
            lblDataPath.Location = new Point(122, 38);
            lblDataPath.Name = "lblDataPath";
            lblDataPath.Size = new Size(75, 24);
            lblDataPath.TabIndex = 0;
            lblDataPath.Text = "D:\\Logs";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 38);
            label1.Name = "label1";
            label1.Size = new Size(100, 24);
            label1.TabIndex = 0;
            label1.Text = "当前路径：";
            // 
            // txtLog
            // 
            txtLog.Dock = DockStyle.Fill;
            txtLog.Location = new Point(3, 26);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(438, 403);
            txtLog.TabIndex = 0;
            // 
            // btnOpenFile
            // 
            btnOpenFile.AutoSize = true;
            btnOpenFile.Location = new Point(217, 85);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(150, 34);
            btnOpenFile.TabIndex = 0;
            btnOpenFile.Text = "查看文件位置";
            btnOpenFile.UseVisualStyleBackColor = true;
            // 
            // btnChangePath
            // 
            btnChangePath.AutoSize = true;
            btnChangePath.Location = new Point(24, 85);
            btnChangePath.Name = "btnChangePath";
            btnChangePath.Size = new Size(150, 34);
            btnChangePath.TabIndex = 0;
            btnChangePath.Text = "变更存放路径";
            btnChangePath.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtLog);
            groupBox2.Dock = DockStyle.Right;
            groupBox2.Location = new Point(436, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(444, 432);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "交互日志";
            // 
            // panel1
            // 
            panel1.Controls.Add(groupBox2);
            panel1.Controls.Add(groupBox1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(880, 432);
            panel1.TabIndex = 2;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(groupBox3);
            groupBox1.Controls.Add(btnSend);
            groupBox1.Controls.Add(btnDisconnect);
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(btnConnect);
            groupBox1.Controls.Add(txtMessage);
            groupBox1.Controls.Add(txtServerPort);
            groupBox1.Controls.Add(txtServerIP);
            groupBox1.Controls.Add(lblStatus);
            groupBox1.Controls.Add(lblIP);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(lblPort);
            groupBox1.Dock = DockStyle.Left;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(429, 432);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "基本配置";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(lblDataPath);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(btnOpenFile);
            groupBox3.Controls.Add(btnChangePath);
            groupBox3.Location = new Point(15, 226);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(395, 144);
            groupBox3.TabIndex = 1;
            groupBox3.TabStop = false;
            groupBox3.Text = "存放路径";
            // 
            // btnSend
            // 
            btnSend.AutoSize = true;
            btnSend.Location = new Point(309, 176);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(101, 34);
            btnSend.TabIndex = 0;
            btnSend.Text = "发送";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(98, 180);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(178, 30);
            txtMessage.TabIndex = 2;
            // 
            // lblIP
            // 
            lblIP.AutoSize = true;
            lblIP.Location = new Point(62, 80);
            lblIP.Name = "lblIP";
            lblIP.Size = new Size(26, 24);
            lblIP.TabIndex = 0;
            lblIP.Text = "IP";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(9, 183);
            label2.Name = "label2";
            label2.Size = new Size(79, 24);
            label2.TabIndex = 0;
            label2.Text = "Content";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new Point(43, 126);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(46, 24);
            lblPort.TabIndex = 0;
            lblPort.Text = "Port";
            // 
            // FormClient
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(880, 432);
            Controls.Add(panel1);
            Name = "FormClient";
            Text = "Form1";
            Load += Form1_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            panel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnDisconnect;
        private Button btnSave;
        private Button btnConnect;
        private TextBox txtServerPort;
        private TextBox txtServerIP;
        private Label lblStatus;
        private Label lblDataPath;
        private Label label1;
        private TextBox txtLog;
        private Button btnOpenFile;
        private Button btnChangePath;
        private GroupBox groupBox2;
        private Panel panel1;
        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private Label lblIP;
        private Label lblPort;
        private Button btnSend;
        private TextBox txtMessage;
        private Label label2;
    }
}
