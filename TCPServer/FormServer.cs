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

            // 当用户点击关闭按钮时
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // 询问用户是否要最小化到托盘或彻底退出
                DialogResult result = MessageBox.Show(
                    "是否将程序最小化到系统托盘？",
                    "StringMonitor",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes) // 最小化到托盘
                {
                    e.Cancel = true;
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                    this.ShowInTaskbar = false;
                }
                else if (result == DialogResult.No) // 彻底退出
                {
                    // 执行清理操作
                    CleanupAndExit();
                }
                else // 取消关闭操作
                {
                    e.Cancel = true;
                }
            }
            else
            {
                // 其他原因关闭(如系统关机)时直接清理并退出
                CleanupAndExit();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 当窗体最小化时，隐藏到托盘
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;

                // 可选：显示通知
                if (!chkSilentStart.Checked && notifyIcon != null) // 静默启动时不显示通知
                {
                    notifyIcon.ShowBalloonTip(3000, "StringMonitor",
                        "应用程序已最小化到系统托盘。", ToolTipIcon.Info);
                }
            }
        }

        private void CleanupAndExit()
        {
            // 停止TCP监听
            isRunning = false;
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }

            // 清理托盘图标
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }

            // 确保应用程序退出
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
                logMessage($"客户端已连接: {clientEndPoint.Address}:{clientEndPoint.Port}");
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
                    logMessage($"客户端断开连接: {clientEndPoint.Address}:{clientEndPoint.Port}");
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
                logMessage($"客户端已连接: {clientEndPoint.Address}:{clientEndPoint.Port}");
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
                            logMessage($"读取客户端 {clientEndPoint.Address}:{clientEndPoint.Port} 数据时发生错误: {ex.Message}");
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

                    // --- 编码检测和解码策略 ---

                    // 1. 尝试使用 Ude.CharsetDetector
                    CharsetDetector cdet = new CharsetDetector();
                    cdet.Feed(buffer, 0, bytesRead);
                    cdet.DataEnd();

                    if (!string.IsNullOrEmpty(cdet.Charset))
                    {
                        try
                        {
                            Encoding detectedEncoding = Encoding.GetEncoding(cdet.Charset);
                            string tempText = detectedEncoding.GetString(buffer, 0, bytesRead);

                            // 启发式检查：解码后的文本是否包含过多的替换字符？
                            if (!ContainsTooManyReplacementChars(tempText, 0.1)) // 10% threshold
                            {
                                receivedText = tempText;
                                finalEncoding = detectedEncoding;
                                // loggerConfig.Debug($"Ude 检测到编码: {cdet.Charset} for {clientEndPoint}");
                            }
                            else
                            {
                                // loggerConfig.Debug($"Ude 检测到 {cdet.Charset}, 但结果包含过多替换字符 for {clientEndPoint}.尝试备选方案.");
                            }
                        }
                        catch (ArgumentException) // Encoding.GetEncoding 可能因不支持的字符集名称抛出异常
                        {
                            // loggerConfig.Warn($"Ude 检测到字符集 '{cdet.Charset}' (for {clientEndPoint}), 但系统不支持此编码. 尝试备选方案.");
                        }
                        //catch (DecoderFallbackException)
                        //{
                        //    // loggerConfig.Warn($"Ude 检测到字符集 '{cdet.Charset}' (for {clientEndPoint}), 但解码失败. 尝试备选方案.");
                        //}
                    }
                    else
                    {
                        // loggerConfig.Debug($"Ude 未能检测到编码 for {clientEndPoint}. 尝试备选方案.");
                    }

                    // 2. 如果 Ude 失败或结果不佳，尝试备选常用编码
                    if (receivedText == null)
                    {
                        // 优先顺序可以根据您的客户端群体进行调整
                        Encoding[] fallbackEncodings = {
                    Encoding.UTF8, // 始终优先尝试 UTF-8
                    // Encoding.Default, // 非常不推荐在服务器端使用，因为它依赖于服务器操作系统的区域设置。
                                     // 如果您确实需要支持特定区域的旧版客户端，请明确指定它们的编码，
                                     // 例如 Encoding.GetEncoding("GBK") 而不是 Encoding.Default。
                    Encoding.GetEncoding("GBK"),         // 简体中文常用
                    Encoding.GetEncoding("Big5"),        // 繁体中文常用
                    Encoding.GetEncoding("Windows-1252"),// 西欧语言常用 (ISO-8859-1 的超集)
                    Encoding.GetEncoding("ISO-8859-1"),  // Latin-1
                    Encoding.Unicode,                    // UTF-16 LE (Windows 内部常用)
                    Encoding.BigEndianUnicode            // UTF-16 BE
                    // 根据需要添加其他可能的编码
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
                                    // loggerConfig.Debug($"使用备选编码成功解码: {enc.EncodingName} for {clientEndPoint}");
                                    break; // 找到合适的编码，跳出循环
                                }
                            }
                            catch (DecoderFallbackException)
                            {
                                // 当前备选编码不适用，继续尝试下一个
                            }
                        }
                    }

                    // 3. 如果所有尝试都失败
                    if (receivedText == null)
                    {
                        // loggerConfig.Error($"无法为来自 {clientEndPoint} 的消息确定编码. 将使用UTF-8并显示替换字符.");
                        // 作为最后的手段，尝试用UTF-8解码，这至少可以显示有效的UTF-8部分和替换字符
                        try
                        {
                            receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            finalEncoding = Encoding.UTF8; // 明确记录我们最终使用的编码
                        }
                        catch (DecoderFallbackException ex) // 即使是UTF-8也可能因为DecoderExceptionFallback失败
                        {
                            receivedText = $"[无法解码的原始数据: {bytesRead} 字节. 解码错误: {ex.Message}]";
                            // loggerConfig.Error($"最终尝试UTF-8解码也失败 for {clientEndPoint}: {ex.Message}");
                            // 此时可以考虑记录原始字节的十六进制形式
                            // string rawHex = BitConverter.ToString(buffer, 0, bytesRead).Replace("-", "");
                            // loggerConfig.Debug($"Raw hex data from {clientEndPoint}: {rawHex}");
                        }
                    }

                    loggerConfig.Trace(receivedText + Environment.NewLine); // 记录原始数据

                    // 日志记录和UI更新
                    string encodingUsedMessage = finalEncoding != null ? finalEncoding.EncodingName : "未知(可能原始字节)";
                    // loggerConfig.Info($"[{clientEndPoint} 使用 {encodingUsedMessage}]: {receivedText}{Environment.NewLine}");

                    // 确保 startTime 在此作用域内有效，或者从其他地方获取
                    Invoke(new Action(() =>
                    {
                        logMessage($"{startTime} [{clientEndPoint.Address}:{clientEndPoint.Port}]: {receivedText}");
                        lblEncodingFormat.Text = $"当前字符串编码格式：{encodingUsedMessage}";

                        // 如果需要发送响应，确保响应也使用对方能理解的编码，或者统一使用UTF-8
                        // SendResponse(clientStream, $"服务器收到: {receivedText}", finalEncoding ?? Encoding.UTF8);
                    }));
                } // end while
            }
            catch (ThreadAbortException)
            {
                // loggerConfig.Info($"线程被中止 for client {clientEndPoint}.");
                // 通常不需要特别处理，线程正在退出
                Thread.ResetAbort(); // 如果您想尝试阻止中止（通常不推荐）
            }
            catch (Exception ex) // 捕获其他未预料的异常
            {
                Invoke(new Action(() =>
                {
                    logMessage($"处理客户端 {clientEndPoint?.Address}:{clientEndPoint?.Port} 时发生严重错误: {ex.ToString()}"); // 使用 ex.ToString() 获取更详细的堆栈信息
                }));
            }
            finally // 确保资源被释放
            {
                // 清理客户端连接
                tcpClient.Close();
                Invoke(new Action(() =>
                {
                    logMessage($"客户端断开连接: {clientEndPoint?.Address}:{clientEndPoint?.Port}");
                }));
            }
        }

        /// <summary>
        /// 检查解码后的文本是否包含过多的Unicode替换字符 (U+FFFD)。
        /// </summary>
        /// <param name="text">要检查的文本。</param>
        /// <param name="thresholdPercentage">替换字符所占比例的阈值（例如0.1表示10%）。</param>
        /// <returns>如果替换字符过多则为true，否则为false。</returns>
        private bool ContainsTooManyReplacementChars(string text, double thresholdPercentage)
        {
            if (string.IsNullOrEmpty(text))
            {
                // 如果原始字节不为空但解码为空字符串，也可能是有问题的
                return false; // 或者根据具体情况判断，例如，如果 bytesRead > 0 而 text 为空，则为 true
            }

            int replacementCharCount = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\uFFFD') // Unicode Replacement Character
                {
                    replacementCharCount++;
                }
            }

            // 对于非常短的文本，即使一个替换字符也可能超过阈值。
            // 可以增加一个绝对数量的判断，例如，如果长度小于10，则允许1个替换字符。
            if (text.Length < 10 && replacementCharCount <= 1) return false;
            if (text.Length == 0) return false; // 避免除以零

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
                        // 如果是文件，选中该文件
                        System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                        processStartInfo.FileName = "explorer.exe";
                        processStartInfo.Arguments = "/select," + path;
                        System.Diagnostics.Process.Start(processStartInfo);
                    }
                    else if (System.IO.Directory.Exists(path))
                    {
                        // 如果是目录，直接打开目录
                        System.Diagnostics.Process.Start("explorer.exe", path);
                    }
                    else
                    {
                        MessageBox.Show("指定的路径不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"打开路径时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #region Tray Icon

        private void InitializeTrayIcon()
        {
            // 创建托盘图标的上下文菜单
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("显示主窗口", null, OnTrayShowWindow);
            trayMenu.Items.Add("启动服务", null, OnTrayStartServer);
            trayMenu.Items.Add("停止服务", null, OnTrayStopServer);
            trayMenu.Items.Add("-"); // 分隔线
            trayMenu.Items.Add("退出", null, OnTrayExit);

            // 初始化托盘图标
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = "StringMonitor TCP服务器";
            notifyIcon.Icon = this.Icon; // 使用程序图标
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
            // 确保清理资源并完全退出应用
            isRunning = false;

            if (tcpListener != null)
            {
                tcpListener.Stop();
            }

            if (notifyIcon != null) // 添加空引用检查
            {
                notifyIcon.Visible = false;
            }

            Application.Exit();
        }

        private void OnTrayDoubleClick(object? sender, EventArgs e)
        {
            // 双击托盘图标显示主窗口
            OnTrayShowWindow(sender, e);
        }

        #endregion

        #region Tooltips

        private void SetupTooltips()
        {
            // 配置ToolTip外观
            toolTip1.AutoPopDelay = 5000;      // 提示显示时间(ms)
            toolTip1.InitialDelay = 500;       // 鼠标悬停多久后显示提示(ms)
            toolTip1.ReshowDelay = 200;        // 从一个控件移动到另一个控件的延迟时间(ms)
            toolTip1.ShowAlways = true;        // 即使窗体不是活动窗体也显示提示
            toolTip1.IsBalloon = true;         // 使用气泡样式（可选）

            toolTip1.SetToolTip(lblInfoIcon_SilentStart, "启动程序时在后台模式运行，不显示主窗口");
            toolTip1.SetToolTip(lblInfoIcon_AutoLaunch, "随系统启动时自动运行程序");
        }

        private void SetupInfoIcons()
        {
            // 创建并配置静默启动的信息图标
            lblInfoIcon_SilentStart.Location = new Point(chkSilentStart.Right + 5, chkSilentStart.Top);

            // 为开机自启动添加信息图标
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
                lblStatus.Text = $"服务器监听中...";
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
                lblStatus.Text = "服务器停止监听";
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
                MessageBox.Show("配置已保存", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
