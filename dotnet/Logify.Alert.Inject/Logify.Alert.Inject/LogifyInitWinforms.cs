extern alias win;
using System;
using System.Threading;
using System.Windows.Forms;
using win::DevExpress.Logify.Win;
using win::DevExpress.Logify.Core.Internal;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace DevExpress.Logify.Core.Internal {
    partial class LogifyInit {
        delegate void InvokeDelegate();

        public static int RunWinForms(string arg) {
            bool isOk = false;
            try {
                //File.WriteAllText(@"C:\Projects\logify_sandbox\LogifyInject\LogifyInject\dbg.log", "LogifyInit.Run");
                const int totalTimeout = 5000;
                const int smallTimeout = 1000;
                int count = totalTimeout / smallTimeout;
                for (int i = 0; i < count; i++) {
                    if (Application.OpenForms == null || Application.OpenForms.Count <= 0)
                        Thread.Sleep(smallTimeout);
                    else {
                        //File.WriteAllText(@"C:\Projects\DevExpress.Logify\Maintenance\LogifyInject\dbg.log", "Win.BeginInvoke");
                        Delegate call = new InvokeDelegate(InitLogifyWinForms);
                        Application.OpenForms[0].BeginInvoke(call);
                        isOk = true;
                        break;
                    }
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

        static void InitLogifyWinForms() {
            try {
                LogifyAlert client = LogifyAlert.Instance;
                ClientConfigurationLoader.ConfigureClientFromFile(client, GetConfigFileName());
                client.StartExceptionsHandling();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "InitLogify exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static string GetConfigFileName() {
            string result;
            Process process = Process.GetCurrentProcess();
            result = Path.Combine(Path.GetDirectoryName(process.MainModule.FileName), "logify.config");
            if (File.Exists(result))
                return result;
            Assembly asm = Assembly.GetExecutingAssembly();
            return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(asm.Location)), "logify.config");
        }
    }
}