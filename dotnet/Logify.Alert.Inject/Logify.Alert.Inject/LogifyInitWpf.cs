extern alias wpf;
using System;
using System.Windows.Threading;
using wpf::DevExpress.Logify.WPF;
using wpf::DevExpress.Logify.Core.Internal;
using System.Windows;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    partial class LogifyInit {
        public static int RunWpf(string arg) {
            try {
                //System.IO.File.WriteAllText(@"C:\Projects\DevExpress.Logify\Maintenance\LogifyInject\dbg.log", "LogifyInit.RunWpf");
                const int totalTimeout = 5000;
                const int smallTimeout = 1000;
                int count = totalTimeout / smallTimeout;
                bool isOk = false;
                for (int i = 0; i < count; i++) {
                    if (TryInitLogifyWpf()) {
                        isOk = true;
                        break;
                    }
                    Thread.Sleep(smallTimeout);
                }
                if (!isOk) {
                    //File.WriteAllText(@"C:\Projects\DevExpress.Logify\Maintenance\LogifyInject\dbg.log", "Win.Direct");
                    InitLogifyWinForms();
                }
                return 0;
            }
            catch {
                return 1;
            }
        }
        static bool TryInitLogifyWpf() {
            Delegate call = new InvokeDelegate(InitLogifyWpf);
            if (Application.Current != null && Application.Current.Dispatcher != null) {
                Application.Current.Dispatcher.BeginInvoke(call, new object[] { });
                return true;
            }
            return false;
        }
        static void InitLogifyWpf() {
            try {
                LogifyAlert client = LogifyAlert.Instance;
                ClientConfigurationLoader.ConfigureClientFromFile(client, GetConfigFileName());
                client.StartExceptionsHandling();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "InitLogify exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}