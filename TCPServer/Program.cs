namespace TCPServer
{
    internal static class Program
    {
        private static Mutex? mutex = null;
        private const string appName = "StringMonitor"; // 确保这个名称的唯一性

        [STAThread]
        static void Main()
        {
            // enable support for additional encodings
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            bool createdNew;
            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // 程序已经在运行
                MessageBox.Show("StringMonitor 程序已经在运行中。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // 可以选择发送消息给已运行的实例，让其显示在最前端
                return; // 直接退出当前尝试启动的实例
            }

            // 如果是新创建的，则正常运行程序
            ApplicationConfiguration.Initialize();
            try
            {
                Application.Run(new FormServer()); // FormServer 构造函数中不再需要 Mutex 相关代码
            }
            finally
            {
                // 当 Application.Run 结束后 (即程序退出时)
                mutex.ReleaseMutex(); // 释放互斥锁
                mutex.Dispose();      // 销毁互斥锁对象
            }
        }
    }
}