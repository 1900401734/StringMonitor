namespace TCPServer
{
    internal static class Program
    {
        private static Mutex? mutex = null;
        private const string appName = "StringMonitor"; // ȷ��������Ƶ�Ψһ��

        [STAThread]
        static void Main()
        {
            // enable support for additional encodings
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            bool createdNew;
            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // �����Ѿ�������
                MessageBox.Show("StringMonitor �����Ѿ��������С�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // ����ѡ������Ϣ�������е�ʵ����������ʾ����ǰ��
                return; // ֱ���˳���ǰ����������ʵ��
            }

            // ������´����ģ����������г���
            ApplicationConfiguration.Initialize();
            try
            {
                Application.Run(new FormServer()); // FormServer ���캯���в�����Ҫ Mutex ��ش���
            }
            finally
            {
                // �� Application.Run ������ (�������˳�ʱ)
                mutex.ReleaseMutex(); // �ͷŻ�����
                mutex.Dispose();      // ���ٻ���������
            }
        }
    }
}