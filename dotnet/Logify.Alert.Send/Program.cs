using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DevExpress.Logify.Core;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Send {
    class Program {
        static Dictionary<string, Action<SenderConfiguration, string>> configHandlers = CreateConfigHandlers();

        static Dictionary<string, Action<SenderConfiguration, string>> CreateConfigHandlers() {
            Dictionary<string, Action<SenderConfiguration, string>> result = new Dictionary<string, Action<SenderConfiguration, string>>();
            result.Add("ApiKey", ReadApiKey);
            result.Add("ServiceUrl", ReadServiceUrl);
            result.Add("ReportFileName", ReadReportFileName);
            result.Add("DeleteReportFile", ReadDeleteReportFile);
            result.Add("DeleteConfigFile", ReadDeleteConfigFile);
            result.Add("MiniDumpGuid", ReadMiniDumpGuid);
            result.Add("MiniDumpFileName", ReadMiniDumpFileName);
            result.Add("MiniDumpServiceUrl", ReadMiniDumpServiceUrl);
            result.Add("Update", ReadUpdate);
            return result;
        }

        static void ReadApiKey(SenderConfiguration sender, string value) {
            sender.ApiKey = value;
        }
        static void ReadServiceUrl(SenderConfiguration sender, string value) {
            sender.Url = value;
        }
        static void ReadReportFileName(SenderConfiguration sender, string value) {
            sender.ReportFileName = value;
        }
        static void ReadDeleteReportFile(SenderConfiguration sender, string value) {
            sender.DeleteReportFile = value;
        }
        static void ReadDeleteConfigFile(SenderConfiguration sender, string value) {
            sender.DeleteConfigFile = value;
        }
        static void ReadMiniDumpGuid(SenderConfiguration sender, string value) {
            sender.MiniDumpGuid = value;
        }
        static void ReadMiniDumpFileName(SenderConfiguration sender, string value) {
            sender.MiniDumpFileName = value;
        }
        static void ReadMiniDumpServiceUrl(SenderConfiguration sender, string value) {
            sender.MiniDumpServiceUrl = value;
        }
        static void ReadUpdate(SenderConfiguration sender, string value) {
            sender.Update = value;
        }
        static int Main(string[] args) {
            Program instance = new Program();
            return instance.Run(args);
        }

        public int Run(string[] args) {
            Console.WriteLine("DevExpress LogifySend utility " + AssemblyInfo.AssemblyCopyright);
            if (args == null || args.Length != 1) {
                PrintUsage();
                return 1;
            }
            try {
                if (String.Compare(args[0], "/U", StringComparison.InvariantCultureIgnoreCase) == 0) {
                    try {
                        Console.WriteLine("Updating client...");
                        AutoUpdater updater = new AutoUpdater();
                        updater.Run("12345678FEE1DEADBEEF4B1DBABEFACE");
                        Console.WriteLine("Complete.");
                    }
                    catch {
                    }
                    return 0;
                }
                return SendReport(args[0]);
            }
            catch {
                return -1;
            }
        }
        public int SendReport(string configFileName) {
            if (!File.Exists(configFileName)) {
                Console.WriteLine("file '" + configFileName + "' not found.");
                return 2;
            }


            SenderConfiguration config = LoadConfiguration(configFileName);
            if (!ValidateConfiguration(config))
                return 2;

            Console.WriteLine("Configuration:");
            Console.WriteLine("ServiceUrl=" + config.Url);
            Console.WriteLine("ApiKey=" + config.ApiKey);
            Console.WriteLine("ReportFileName=" + config.ReportFileName);
            Console.WriteLine("MiniDumpGuid=" + config.MiniDumpGuid);
            Console.WriteLine("MiniDumpFileName=" + config.MiniDumpFileName);
            Console.WriteLine("MiniDumpServiceUrl=" + config.MiniDumpServiceUrl);

            InnerExceptionReportSender sender = new InnerExceptionReportSender();
            sender.ServiceUrl = config.Url;
            sender.ApiKey = config.ApiKey;
            string report = File.ReadAllText(config.ReportFileName, Encoding.UTF8);

            Console.WriteLine("Sending report...");
            if (!String.IsNullOrEmpty(config.MiniDumpGuid) && !String.IsNullOrEmpty(config.MiniDumpFileName)) {
                sender.SendExceptionReport(report);
                //TODO:
                //AM: use send protocol with confirmation, to avoid duplicate minidump send
                //AM: send minidump, if report is not dup
                MiniDumpSender dumpSender = new MiniDumpSender(config.MiniDumpServiceUrl);
                dumpSender.UploadWithConfirmation(config.ApiKey, config.MiniDumpGuid, config.MiniDumpFileName);
            }
            else
                sender.SendExceptionReport(report);
            if (!String.IsNullOrEmpty(config.DeleteReportFile) && !String.IsNullOrEmpty(config.DeleteReportFile.Trim())) {
                try {
                    File.Delete(config.ReportFileName);
                }
                catch {
                }
            }
            if (!String.IsNullOrEmpty(config.DeleteConfigFile) && !String.IsNullOrEmpty(config.DeleteConfigFile.Trim())) {
                try {
                    File.Delete(configFileName);
                }
                catch {
                }
            }
            if (!String.IsNullOrEmpty(config.MiniDumpFileName) && File.Exists(config.MiniDumpFileName.Trim())) {
                try {
                    File.Delete(config.MiniDumpFileName.Trim());
                }
                catch {
                }
            }
            Console.WriteLine("Complete.");

            if (!String.IsNullOrEmpty(config.Update)) {
                try {
                    Console.WriteLine("Updating client...");
                    AutoUpdater updater = new AutoUpdater();
                    updater.Run(config.ApiKey);
                    Console.WriteLine("Complete.");
                }
                catch {
                }
            }
            
            return 0;
        }

        bool ValidateConfiguration(SenderConfiguration config) {
            if (String.IsNullOrEmpty(config.Url)) {
                Console.WriteLine("ERROR: service Url not specified");
                return false;
            }
            if (String.IsNullOrEmpty(config.ApiKey)) {
                Console.WriteLine("ERROR: ApiKey not specified");
                return false;
            }
            if (String.IsNullOrEmpty(config.ReportFileName)) {
                Console.WriteLine("ERROR: ReportFileName not specified");
                return false;
            }
            if (!File.Exists(config.ReportFileName)) {
                Console.WriteLine("ERROR: file '" + config.ReportFileName + "' not found");
                return false;
            }
            return true;
        }
        SenderConfiguration LoadConfiguration(string fileName) {
            Console.WriteLine("Loading configuration from file '" + fileName + "'");
            string[] lines = File.ReadAllLines(fileName);
            SenderConfiguration result = new SenderConfiguration();
            foreach (string line in lines)
                ProcessConfigLine(line, result);
            return result;
        }

        void ProcessConfigLine(string line, SenderConfiguration result) {
            if (String.IsNullOrEmpty(line))
                return;
            string[] parts = line.Split('=');
            if (parts == null || parts.Length != 2)
                return;

            string parameterName = parts[0].Trim();
            Action<SenderConfiguration, string> handler;
            if (configHandlers.TryGetValue(parameterName, out handler)) {
                string value = parts[1].Trim();
                handler(result, value);
            }
        }
        void PrintUsage() {
            Console.WriteLine("Usage:");
            Console.WriteLine("    LogifySend.exe <report file>");
        }
    }

    public class SenderConfiguration {
        public string Url { get; set; }
        public string ApiKey { get; set; }
        public string ReportFileName { get; set; }
        public string DeleteReportFile { get; set; }
        public string DeleteConfigFile { get; set; }
        public string MiniDumpGuid { get; set; }
        public string MiniDumpFileName { get; set; }
        public string MiniDumpServiceUrl { get; set; }
        public string Update { get; set; }
    }
}
