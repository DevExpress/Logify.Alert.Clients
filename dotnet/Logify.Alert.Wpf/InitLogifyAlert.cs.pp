using System;
using System.IO;
using System.Text;
using DevExpress.Logify.Core;
using DevExpress.Logify.WPF;

namespace $rootnamespace$ {
    public static class LogifyAlertInit {
        static DateTime startTime = DateTime.Now;

        public static void Run() {
            
            LogifyAlert client = LogifyAlert.Instance;
            
            // Values set here will override ones specified in the app.config file
            //client.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
            //client.AppName = "$assemblyname$";
            //client.AppVersion = "1.0.0";
            
            client.CanReportException += CanReportException;
            client.BeforeReportException += BeforeReportException;

            client.UserId = System.Environment.UserName;
            
            // Adds custom data to a report
            client.CustomData["MACHINE_NAME"] = System.Environment.MachineName;
            
            // Uncomment this code to attach a last opened file to a report
            // client.Attachments.Add(new Attachment() {
                // Name = "lastOpenFile.bin",
                // MimeType = "application/octet-stream",
                // Content = GetLastOpenFileBytes()
            //});

            // Keeps OfflineReportsCount crash reports within the specified directory if you're offline
            client.OfflineReportsEnabled = false;
            client.OfflineReportsDirectory = "OfflineReportsFolder";
            client.OfflineReportsCount = 10;
            if (client.OfflineReportsEnabled && !Directory.Exists(client.OfflineReportsDirectory)) {
                Directory.CreateDirectory(client.OfflineReportsDirectory);
            }

            client.StartExceptionsHandling();
        }

        static void CanReportException(object sender, CanReportExceptionEventArgs args) {
            #if DEBUG
                args.Cancel = true; // Doesn't allow sending reports during debugging
            #endif
        }
        
        // Executes this code before sending a report
        static void BeforeReportException(object sender, BeforeReportExceptionEventArgs args) {
            LogifyAlert client = LogifyAlert.Instance;
            client.CustomData["UpTime"] = (DateTime.Now - startTime).ToString(); 
        }
    }
}