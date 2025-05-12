using System.Net.Sockets;
using System.Text;

namespace TCPClient
{
    public partial class FormClient : Form
    {
        private TcpClient? tcpClient;
        private NetworkStream? clientStream;
        private string serverIP = "127.0.0.1";
        private int serverPort = 9000;
        private bool isConnected = false;

        public FormClient()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadConfiguration();

            // Initialize UI
            lblStatus.Text = "Disconnected";
            btnDisconnect.Enabled = false;
            btnSend.Enabled = false;
            this.Text = "TCP Client";
        }

        private void LoadConfiguration()
        {
            try
            {
                string configPath = System.IO.Path.Combine(Application.StartupPath, "config.ini");
                if (System.IO.File.Exists(configPath))
                {
                    string[] lines = System.IO.File.ReadAllLines(configPath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            switch (key)
                            {
                                case "IP":
                                    txtServerIP.Text = value;
                                    break;
                                case "Port":
                                    txtServerPort.Text = value;
                                    break;
                                case "DataPath":
                                    lblDataPath.Text = value;
                                    break;
                            }
                        }
                    }
                    logMessage("配置已加载");

                }
                else
                {
                    lblDataPath.Text = "D:\\Log";
                }
            }
            catch (Exception ex)
            {
                logMessage($"加载配置失败: {ex.Message}");
            }
        }

        private void ReceiveMessages()
        {
            byte[] message = new byte[4096];
            int bytesRead;

            try
            {
                while (isConnected)
                {
                    bytesRead = 0;

                    // Ensure clientStream and tcpClient are not null before accessing them
                    if (clientStream != null && tcpClient != null && clientStream.CanRead && tcpClient.Connected)
                    {
                        bytesRead = clientStream.Read(message, 0, 4096);
                    }
                    else
                    {
                        // Connection lost
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        // Connection closed by server
                        break;
                    }

                    // Process received message
                    string receivedMessage = Encoding.UTF8.GetString(message, 0, bytesRead);

                    // Update UI
                    Invoke(new Action(() =>
                    {
                        logMessage($"Received: {receivedMessage}");
                    }));
                }

                // If we exit the loop, connection is closed
                Invoke(new Action(() =>
                {
                    logMessage("Connection closed by server");
                    CleanupConnection();
                }));
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    logMessage($"Connection error: {ex.Message}");
                    CleanupConnection();
                }));
            }
        }

        private void CleanupConnection()
        {
            // Reset connection status
            isConnected = false;

            // Close network resources
            if (clientStream != null)
            {
                clientStream.Close();
                clientStream = null;
            }

            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }

            // Update UI
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => lblStatus.Text = "Disconnected"));
                btnConnect.Invoke(new Action(() => btnConnect.Enabled = true));
                btnDisconnect.Invoke(new Action(() => btnDisconnect.Enabled = false));
                btnSend.Invoke(new Action(() => btnSend.Enabled = false));
                txtServerIP.Invoke(new Action(() => txtServerIP.Enabled = true));
                txtServerPort.Invoke(new Action(() => txtServerPort.Enabled = true));
            }
            else
            {
                lblStatus.Text = "Disconnected";
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                btnSend.Enabled = false;
                txtServerIP.Enabled = true;
                txtServerPort.Enabled = true;
            }
        }

        private void logMessage(string message)
        {
            // Thread-safe logging
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(logMessage), message);
                return;
            }

            txtLog.AppendText(message + Environment.NewLine);
            // Auto-scroll to bottom
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        #region Event Handlers

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string configPath = System.IO.Path.Combine(Application.StartupPath, "config.ini");
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(configPath))
                {
                    // 保存IP地址（从代码看您需要添加一个txtIP文本框）
                    writer.WriteLine($"IP={txtServerIP.Text}");
                    // 保存端口
                    writer.WriteLine($"Port={txtServerPort.Text}");
                    // 保存数据存放路径
                    writer.WriteLine($"DataPath={lblDataPath.Text}");
                }

                MessageBox.Show("配置已保存", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // Get server details from UI
                if (!string.IsNullOrEmpty(txtServerIP.Text))
                    serverIP = txtServerIP.Text;

                if (!string.IsNullOrEmpty(txtServerPort.Text))
                    serverPort = int.Parse(txtServerPort.Text);

                // Create TCP client and connect
                tcpClient = new TcpClient();
                tcpClient.Connect(serverIP, serverPort);

                // Get stream for reading and writing
                clientStream = tcpClient.GetStream();

                // Update connection status
                isConnected = true;
                lblStatus.Text = $"Connected to {serverIP}:{serverPort}";
                logMessage($"Connected to server at {DateTime.Now}");

                // Update UI
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                btnSend.Enabled = true;
                txtServerIP.Enabled = false;
                txtServerPort.Enabled = false;

                // Set up background thread for receiving messages
                System.Threading.Thread receiveThread = new System.Threading.Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                CleanupConnection();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            CleanupConnection();
            logMessage("Disconnected from server");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (isConnected && !string.IsNullOrEmpty(txtMessage.Text))
                {
                    // Convert message to bytes
                    byte[] messageBytes = Encoding.UTF8.GetBytes(txtMessage.Text);

                    // Send message to server
                    if (clientStream != null)
                    {
                        clientStream.Write(messageBytes, 0, messageBytes.Length);
                        clientStream.Flush();
                    }

                    // Log sent message
                    logMessage($"Sent: {txtMessage.Text}");

                    // Clear message text box
                    txtMessage.Clear();
                }
            }
            catch (Exception ex)
            {
                logMessage($"Error sending message: {ex.Message}");
                CleanupConnection();
            }
        }

        #endregion
    }
}
