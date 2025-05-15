using NLog;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Ude;

namespace TCPServer
{
    public partial class FormServer : Form
    {
        // TCP server properties
        private TcpListener? tcpListener;
        private Thread? listenerThread;
        private Logger loggerConfig = LogManager.GetLogger("DataLog");
        private int port = 9000;
        private bool isRunning = false;
        private NotifyIcon? notifyIcon;
        private ContextMenuStrip? trayMenu;


        // Comment

        public FormServer()
        {
            InitializeComponent();
            LoadConfiguration();
            LogManager.Configuration.Variables["logDirectory"] = lblDataPath.Text;
            this.Text = "StringMonitor";

            InitializeTrayIcon();
            SetupTooltips();
            SetupInfoIcons();

            if (chkSilentStart.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Visible = false;
                btnStart_Click(null, null);
            }
        }

        private void FormServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure clean shutdown when the form is closed
            //isRunning = false;
            //if (tcpListener != null)
            //{
            //    tcpListener.Stop();
            //}

            // ���û�����رհ�ťʱ
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // ѯ���û��Ƿ�Ҫ��С�������̻򳹵��˳�
                DialogResult result = MessageBox.Show(
                    "�Ƿ񽫳�����С����ϵͳ���̣�",
                    "StringMonitor",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes) // ��С��������
                {
                    e.Cancel = true;
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                    this.ShowInTaskbar = false;
                }
                else if (result == DialogResult.No) // �����˳�
                {
                    // ִ���������
                    CleanupAndExit();
                }
                else // ȡ���رղ���
                {
                    e.Cancel = true;
                }
            }
            else
            {
                // ����ԭ��ر�(��ϵͳ�ػ�)ʱֱ�������˳�
                CleanupAndExit();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // ��������С��ʱ�����ص�����
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;

                // ��ѡ����ʾ֪ͨ
                if (!chkSilentStart.Checked && notifyIcon != null) // ��Ĭ����ʱ����ʾ֪ͨ
                {
                    notifyIcon.ShowBalloonTip(3000, "StringMonitor",
                        "Ӧ�ó�������С����ϵͳ���̡�", ToolTipIcon.Info);
                }
            }
        }

        private void CleanupAndExit()
        {
            // ֹͣTCP����
            isRunning = false;
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }

            // ��������ͼ��
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }

            // ȷ��Ӧ�ó����˳�
            Application.Exit();
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
                                    txtIP.Text = value;
                                    break;
                                case "Port":
                                    txtPort.Text = value;
                                    break;
                                case "DataPath":
                                    lblDataPath.Text = value;
                                    break;
                                case "IsAutoStart":
                                    chkAutoLaunch.Checked = bool.Parse(value);
                                    break;
                                case "IsSilentStart":
                                    chkSilentStart.Checked = bool.Parse(value);
                                    break;
                            }
                        }
                    }

                    logMessage("�����Ѽ���");
                }
                else
                {
                    lblDataPath.Text = "D:\\Log";
                }
            }
            catch (Exception ex)
            {
                logMessage($"��������ʧ��: {ex.Message}");
            }
        }

        private void ListenForClients()
        {
            try
            {
                // Create and start TCP listener
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();

                // Keep accepting clients while the server is running
                while (isRunning)
                {
                    // Accept client connection (blocks until client connects)
                    TcpClient client = tcpListener.AcceptTcpClient();

                    // Handle client in a new thread
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                }
            }
            catch (SocketException ex)
            {
                // If server stopped normally, this is expected
                if (isRunning)
                {
                    Invoke(new Action(() =>
                    {
                        logMessage($"Listener error: {ex.Message}");
                    }));
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    logMessage($"Error in listener thread: {ex.Message}");
                }));
            }
        }

        /*private void HandleClientComm(object? client)
        {
            if (client is null)
            {
                return;
            }

            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            IPEndPoint? clientEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;

            if (clientEndPoint == null)
            {
                Invoke(new Action(() =>
                {
                    logMessage($"Client endpoint is null. Unable to log client connection.");
                }));
                tcpClient.Close();
                return;
            }

            // Log client connection
            Invoke(new Action(() =>
            {
                logMessage($"�ͻ���������: {clientEndPoint.Address}:{clientEndPoint.Port}");
            }));

            byte[] message = new byte[4096];
            int bytesRead;

            try
            {
                // Process incoming messages while client is connected
                while (isRunning && tcpClient.Connected)
                {
                    bytesRead = 0;

                    // Wait for client data (blocks until data is sent)
                    try
                    {
                        bytesRead = clientStream.Read(message, 0, 4096);
                    }
                    catch
                    {
                        // Socket error - client disconnected
                        break;
                    }

                    // Client disconnected
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    // Process received data
                    cdet = new Ude.CharsetDetector();
                    cdet.Feed(message, 0, bytesRead);
                    cdet.DataEnd();
                    string receivedText = string.Empty;

                    if (cdet.Charset != null)
                    {
                        Encoding detectedEncoding = Encoding.GetEncoding(cdet.Charset);
                        receivedText = detectedEncoding.GetString(message, 0, bytesRead);
                        //string receivedText = Encoding.UTF8.GetString(message, 0, bytesRead);
                        loggerConfig.Info(receivedText + Environment.NewLine);
                    }
                    else
                    {
                        Console.WriteLine("Detection failed.");
                    }

                    // Update UI with received message
                    Invoke(new Action(() =>
                    {
                        logMessage($"{startTime}: {receivedText}");

                        //SendResponse(clientStream, $"Server received: {receivedText}");
                    }));
                }

                // Clean up when client disconnects
                tcpClient.Close();
                Invoke(new Action(() =>
                {
                    logMessage($"�ͻ��˶Ͽ�����: {clientEndPoint.Address}:{clientEndPoint.Port}");
                }));
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    logMessage($"Error handling client: {ex.Message}");
                }));

                // Clean up
                tcpClient.Close();
            }
        }*/

        #region Character Encoding Detection and Processing

        private void HandleClientComm(object? clientObj) // Renamed parameter to avoid conflict
        {
            if (clientObj is not TcpClient tcpClient) // Modern C# pattern matching
            {
                return;
            }

            NetworkStream clientStream = tcpClient.GetStream();
            IPEndPoint? clientEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;

            if (clientEndPoint == null)
            {
                Invoke(new Action(() =>
                {
                    logMessage($"Client endpoint is null. Unable to log client connection.");
                }));
                tcpClient.Close();
                return;
            }

            // Log client connection
            // string startTime = DateTime.Now.ToLongTimeString(); // Assuming startTime is defined elsewhere or passed
            string startTime = DateTime.Now.ToString("HH:mm:ss"); // Example for startTime if needed here

            Invoke(new Action(() =>
            {
                logMessage($"�ͻ���������: {clientEndPoint.Address}:{clientEndPoint.Port}");
            }));

            byte[] buffer = new byte[4096]; // Renamed 'message' to 'buffer' to avoid confusion with 'receivedText'
            int bytesRead;

            // It's good practice to reuse the CharsetDetector instance if you are processing
            // multiple chunks that belong to the same logical stream and you suspect the encoding
            // is consistent. However, if each Read is a new, independent message from potentially
            // different clients or with different encodings, creating a new one per message is fine.
            // For this example, we create it per received data chunk.

            try
            {
                while (isRunning && tcpClient.Connected) // Assuming isRunning is a class member bool
                {
                    bytesRead = 0;
                    try
                    {
                        // Consider adding ReadTimeout if blocking indefinitely is not desired
                        // clientStream.ReadTimeout = 5000; // 5 seconds for example
                        bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                    }
                    catch (IOException ex) when (ex.InnerException is SocketException se && se.SocketErrorCode == SocketError.TimedOut)
                    {
                        // Handle ReadTimeout if you set one
                        // loggerConfig.Debug("Read timeout, no data from client.");
                        continue; // Continue loop to check isRunning and Connected
                    }
                    catch (Exception ex) // More general socket or stream errors
                    {
                        Invoke(new Action(() =>
                        {
                            logMessage($"��ȡ�ͻ��� {clientEndPoint.Address}:{clientEndPoint.Port} ����ʱ��������: {ex.Message}");
                        }));
                        break; // Exit loop on error
                    }

                    if (bytesRead == 0)
                    {
                        // Graceful disconnect by client
                        break;
                    }

                    string? receivedText = null;
                    Encoding? finalEncoding = null;

                    // --- ������ͽ������ ---

                    // 1. ����ʹ�� Ude.CharsetDetector
                    CharsetDetector cdet = new CharsetDetector();
                    cdet.Feed(buffer, 0, bytesRead);
                    cdet.DataEnd();

                    if (!string.IsNullOrEmpty(cdet.Charset))
                    {
                        try
                        {
                            Encoding detectedEncoding = Encoding.GetEncoding(cdet.Charset);
                            string tempText = detectedEncoding.GetString(buffer, 0, bytesRead);

                            // ����ʽ��飺�������ı��Ƿ����������滻�ַ���
                            if (!ContainsTooManyReplacementChars(tempText, 0.1)) // 10% threshold
                            {
                                receivedText = tempText;
                                finalEncoding = detectedEncoding;
                                // loggerConfig.Debug($"Ude ��⵽����: {cdet.Charset} for {clientEndPoint}");
                            }
                            else
                            {
                                // loggerConfig.Debug($"Ude ��⵽ {cdet.Charset}, ��������������滻�ַ� for {clientEndPoint}.���Ա�ѡ����.");
                            }
                        }
                        catch (ArgumentException) // Encoding.GetEncoding ������֧�ֵ��ַ��������׳��쳣
                        {
                            // loggerConfig.Warn($"Ude ��⵽�ַ��� '{cdet.Charset}' (for {clientEndPoint}), ��ϵͳ��֧�ִ˱���. ���Ա�ѡ����.");
                        }
                        //catch (DecoderFallbackException)
                        //{
                        //    // loggerConfig.Warn($"Ude ��⵽�ַ��� '{cdet.Charset}' (for {clientEndPoint}), ������ʧ��. ���Ա�ѡ����.");
                        //}
                    }
                    else
                    {
                        // loggerConfig.Debug($"Ude δ�ܼ�⵽���� for {clientEndPoint}. ���Ա�ѡ����.");
                    }

                    // 2. ��� Ude ʧ�ܻ������ѣ����Ա�ѡ���ñ���
                    if (receivedText == null)
                    {
                        // ����˳����Ը������Ŀͻ���Ⱥ����е���
                        Encoding[] fallbackEncodings = {
                    Encoding.UTF8, // ʼ�����ȳ��� UTF-8
                    // Encoding.Default, // �ǳ����Ƽ��ڷ�������ʹ�ã���Ϊ�������ڷ���������ϵͳ���������á�
                                     // �����ȷʵ��Ҫ֧���ض�����ľɰ�ͻ��ˣ�����ȷָ�����ǵı��룬
                                     // ���� Encoding.GetEncoding("GBK") ������ Encoding.Default��
                    Encoding.GetEncoding("GBK"),         // �������ĳ���
                    Encoding.GetEncoding("Big5"),        // �������ĳ���
                    Encoding.GetEncoding("Windows-1252"),// ��ŷ���Գ��� (ISO-8859-1 �ĳ���)
                    Encoding.GetEncoding("ISO-8859-1"),  // Latin-1
                    Encoding.Unicode,                    // UTF-16 LE (Windows �ڲ�����)
                    Encoding.BigEndianUnicode            // UTF-16 BE
                    // ������Ҫ����������ܵı���
                };

                        foreach (Encoding enc in fallbackEncodings)
                        {
                            try
                            {
                                string tempText = enc.GetString(buffer, 0, bytesRead);
                                if (!ContainsTooManyReplacementChars(tempText, 0.1))
                                {
                                    receivedText = tempText;
                                    finalEncoding = enc;
                                    // loggerConfig.Debug($"ʹ�ñ�ѡ����ɹ�����: {enc.EncodingName} for {clientEndPoint}");
                                    break; // �ҵ����ʵı��룬����ѭ��
                                }
                            }
                            catch (DecoderFallbackException)
                            {
                                // ��ǰ��ѡ���벻���ã�����������һ��
                            }
                        }
                    }

                    // 3. ������г��Զ�ʧ��
                    if (receivedText == null)
                    {
                        // loggerConfig.Error($"�޷�Ϊ���� {clientEndPoint} ����Ϣȷ������. ��ʹ��UTF-8����ʾ�滻�ַ�.");
                        // ��Ϊ�����ֶΣ�������UTF-8���룬�����ٿ�����ʾ��Ч��UTF-8���ֺ��滻�ַ�
                        try
                        {
                            receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            finalEncoding = Encoding.UTF8; // ��ȷ��¼��������ʹ�õı���
                        }
                        catch (DecoderFallbackException ex) // ��ʹ��UTF-8Ҳ������ΪDecoderExceptionFallbackʧ��
                        {
                            receivedText = $"[�޷������ԭʼ����: {bytesRead} �ֽ�. �������: {ex.Message}]";
                            // loggerConfig.Error($"���ճ���UTF-8����Ҳʧ�� for {clientEndPoint}: {ex.Message}");
                            // ��ʱ���Կ��Ǽ�¼ԭʼ�ֽڵ�ʮ��������ʽ
                            // string rawHex = BitConverter.ToString(buffer, 0, bytesRead).Replace("-", "");
                            // loggerConfig.Debug($"Raw hex data from {clientEndPoint}: {rawHex}");
                        }
                    }

                    loggerConfig.Trace(receivedText + Environment.NewLine); // ��¼ԭʼ����

                    // ��־��¼��UI����
                    string encodingUsedMessage = finalEncoding != null ? finalEncoding.EncodingName : "δ֪(����ԭʼ�ֽ�)";
                    // loggerConfig.Info($"[{clientEndPoint} ʹ�� {encodingUsedMessage}]: {receivedText}{Environment.NewLine}");

                    // ȷ�� startTime �ڴ�����������Ч�����ߴ������ط���ȡ
                    Invoke(new Action(() =>
                    {
                        logMessage($"{startTime} [{clientEndPoint.Address}:{clientEndPoint.Port}]: {receivedText}");
                        lblEncodingFormat.Text = $"��ǰ�ַ��������ʽ��{encodingUsedMessage}";

                        // �����Ҫ������Ӧ��ȷ����ӦҲʹ�öԷ������ı��룬����ͳһʹ��UTF-8
                        // SendResponse(clientStream, $"�������յ�: {receivedText}", finalEncoding ?? Encoding.UTF8);
                    }));
                } // end while
            }
            catch (ThreadAbortException)
            {
                // loggerConfig.Info($"�̱߳���ֹ for client {clientEndPoint}.");
                // ͨ������Ҫ�ر����߳������˳�
                Thread.ResetAbort(); // ������볢����ֹ��ֹ��ͨ�����Ƽ���
            }
            catch (Exception ex) // ��������δԤ�ϵ��쳣
            {
                Invoke(new Action(() =>
                {
                    logMessage($"����ͻ��� {clientEndPoint?.Address}:{clientEndPoint?.Port} ʱ�������ش���: {ex.ToString()}"); // ʹ�� ex.ToString() ��ȡ����ϸ�Ķ�ջ��Ϣ
                }));
            }
            finally // ȷ����Դ���ͷ�
            {
                // ����ͻ�������
                tcpClient.Close();
                Invoke(new Action(() =>
                {
                    logMessage($"�ͻ��˶Ͽ�����: {clientEndPoint?.Address}:{clientEndPoint?.Port}");
                }));
            }
        }

        /// <summary>
        /// ���������ı��Ƿ���������Unicode�滻�ַ� (U+FFFD)��
        /// </summary>
        /// <param name="text">Ҫ�����ı���</param>
        /// <param name="thresholdPercentage">�滻�ַ���ռ��������ֵ������0.1��ʾ10%����</param>
        /// <returns>����滻�ַ�������Ϊtrue������Ϊfalse��</returns>
        private bool ContainsTooManyReplacementChars(string text, double thresholdPercentage)
        {
            if (string.IsNullOrEmpty(text))
            {
                // ���ԭʼ�ֽڲ�Ϊ�յ�����Ϊ���ַ�����Ҳ�������������
                return false; // ���߸��ݾ�������жϣ����磬��� bytesRead > 0 �� text Ϊ�գ���Ϊ true
            }

            int replacementCharCount = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\uFFFD') // Unicode Replacement Character
                {
                    replacementCharCount++;
                }
            }

            // ���ڷǳ��̵��ı�����ʹһ���滻�ַ�Ҳ���ܳ�����ֵ��
            // ��������һ�������������жϣ����磬�������С��10��������1���滻�ַ���
            if (text.Length < 10 && replacementCharCount <= 1) return false;
            if (text.Length == 0) return false; // ���������

            return (double)replacementCharCount / text.Length > thresholdPercentage;
        }

        #endregion

        private void SendResponse(NetworkStream stream, string response)
        {
            try
            {
                // Convert string to byte array
                byte[] responseData = Encoding.UTF8.GetBytes(response);

                // Send response back to client
                stream.Write(responseData, 0, responseData.Length);
                stream.Flush();

                // Log the response
                logMessage($"Sent response: {response}");
            }
            catch (Exception ex)
            {
                logMessage($"Error sending response: {ex.Message}");
            }
        }

        private void logMessage(string message)
        {
            // Thread-safe logging to text box
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(logMessage), message);
                return;
            }

            txtLog.AppendText(message + Environment.NewLine);

            // Auto-scroll to the bottom
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void ShowFileInExplorer(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        // ������ļ���ѡ�и��ļ�
                        System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                        processStartInfo.FileName = "explorer.exe";
                        processStartInfo.Arguments = "/select," + path;
                        System.Diagnostics.Process.Start(processStartInfo);
                    }
                    else if (System.IO.Directory.Exists(path))
                    {
                        // �����Ŀ¼��ֱ�Ӵ�Ŀ¼
                        System.Diagnostics.Process.Start("explorer.exe", path);
                    }
                    else
                    {
                        MessageBox.Show("ָ����·�������ڡ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"��·��ʱ��������{ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #region Tray Icon

        private void InitializeTrayIcon()
        {
            // ��������ͼ��������Ĳ˵�
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("��ʾ������", null, OnTrayShowWindow);
            trayMenu.Items.Add("��������", null, OnTrayStartServer);
            trayMenu.Items.Add("ֹͣ����", null, OnTrayStopServer);
            trayMenu.Items.Add("-"); // �ָ���
            trayMenu.Items.Add("�˳�", null, OnTrayExit);

            // ��ʼ������ͼ��
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = "StringMonitor TCP������";
            notifyIcon.Icon = this.Icon; // ʹ�ó���ͼ��
            notifyIcon.ContextMenuStrip = trayMenu;
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += OnTrayDoubleClick;
        }

        private void OnTrayShowWindow(object? sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void OnTrayStartServer(object? sender, EventArgs e)
        {
            if (!isRunning)
            {
                btnStart_Click(null, null);
            }
        }

        private void OnTrayStopServer(object? sender, EventArgs e)
        {
            if (isRunning)
            {
                btnStop_Click(null, null);
            }
        }

        private void OnTrayExit(object? sender, EventArgs e)
        {
            // ȷ��������Դ����ȫ�˳�Ӧ��
            isRunning = false;

            if (tcpListener != null)
            {
                tcpListener.Stop();
            }

            if (notifyIcon != null) // ��ӿ����ü��
            {
                notifyIcon.Visible = false;
            }

            Application.Exit();
        }

        private void OnTrayDoubleClick(object? sender, EventArgs e)
        {
            // ˫������ͼ����ʾ������
            OnTrayShowWindow(sender, e);
        }

        #endregion

        #region Tooltips

        private void SetupTooltips()
        {
            // ����ToolTip���
            toolTip1.AutoPopDelay = 5000;      // ��ʾ��ʾʱ��(ms)
            toolTip1.InitialDelay = 500;       // �����ͣ��ú���ʾ��ʾ(ms)
            toolTip1.ReshowDelay = 200;        // ��һ���ؼ��ƶ�����һ���ؼ����ӳ�ʱ��(ms)
            toolTip1.ShowAlways = true;        // ��ʹ���岻�ǻ����Ҳ��ʾ��ʾ
            toolTip1.IsBalloon = true;         // ʹ��������ʽ����ѡ��

            toolTip1.SetToolTip(lblInfoIcon_SilentStart, "��������ʱ�ں�̨ģʽ���У�����ʾ������");
            toolTip1.SetToolTip(lblInfoIcon_AutoLaunch, "��ϵͳ����ʱ�Զ����г���");
        }

        private void SetupInfoIcons()
        {
            // ���������þ�Ĭ��������Ϣͼ��
            lblInfoIcon_SilentStart.Location = new Point(chkSilentStart.Right + 5, chkSilentStart.Top);

            // Ϊ���������������Ϣͼ��
            lblInfoIcon_AutoLaunch.Location = new Point(chkAutoLaunch.Right + 5, chkAutoLaunch.Top);
        }

        #endregion

        #region Event Handlers

        private void btnStart_Click(object? sender, EventArgs? e)
        {
            try
            {
                // Get port from textbox if available
                if (!string.IsNullOrEmpty(txtPort.Text))
                {
                    port = int.Parse(txtPort.Text);
                }

                // Start server in a new thread
                isRunning = true;
                listenerThread = new Thread(new ThreadStart(ListenForClients));
                listenerThread.IsBackground = true;
                listenerThread.Start();

                // Update UI
                lblStatus.Text = $"������������...";
                lblStatus.ForeColor = Color.Green;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                txtIP.Enabled = false;
                txtPort.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object? sender, EventArgs? e)
        {
            try
            {
                // Stop the server
                isRunning = false;

                if (tcpListener != null)
                {
                    tcpListener.Stop();
                }

                // Update UI
                lblStatus.Text = "������ֹͣ����";
                lblStatus.ForeColor = Color.Red;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                txtIP.Enabled = true;
                txtPort.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping server: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChangePath_Click(object sender, EventArgs e)
        {
            string prePath = lblDataPath.Text;
            FolderBrowserDialog path = new FolderBrowserDialog();

            if (path.ShowDialog() == DialogResult.OK)
            {
                this.lblDataPath.Text = path.SelectedPath;
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            string path = lblDataPath.Text;
            ShowFileInExplorer(path);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string configPath = System.IO.Path.Combine(Application.StartupPath, "config.ini");
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(configPath))
                {
                    writer.WriteLine($"IP={txtIP.Text}");
                    writer.WriteLine($"Port={txtPort.Text}");
                    writer.WriteLine($"DataPath={lblDataPath.Text}");
                    writer.WriteLine($"IsAutoStart={chkAutoLaunch.Checked}");
                    writer.WriteLine($"IsSilentStart={chkSilentStart.Checked}");
                }
                //AutoStart(chkAutoLaunch.Checked);
                AutoLaunch.AutoStart(chkAutoLaunch.Checked);
                MessageBox.Show("�����ѱ���", "�ɹ�", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"��������ʧ��: {ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
