using log4net.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace DevExpress.Logify.Core.Internal {
    class Program {
        static void Main(string[] args) {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

            PrintHello();
            CommandLineConfiguration config = CreateCommandLineConfiguration();
            if (args == null || args.Length <= 0) {
                PrintUsage(config);
                return;
            }

            
            Dictionary<string, CommandLineSwitch> switches = new CommandLineParser().Parse(args, config);
            RunnerParameters parameters = AnalyzeCommandLineSwitches(switches, args);
            if (parameters == null)
                return;

            Process process = AttachToTargetProcess(parameters);
            if (process == null)
                return;

            if (!IsCompatibleProcess(process))
                return;

            CopyLogifyAssembliesToTargetProcessDirectory(process.MainModule.FileName);

            InjectLogifyAssembly(process, process.Id, parameters.IsWinforms);

            //for (;;) {
            //    Console.WriteLine("Press 'Q' to exit");
            //    ConsoleKeyInfo key = Console.ReadKey();
            //    if (key.Key == ConsoleKey.Q)
            //        break;
            //}
        }

        static void CopyLogifyAssembliesToTargetProcessDirectory(string executableFileName) {
            try {
                string targetDirectory = Path.GetDirectoryName(Path.GetFullPath(executableFileName));
                string sourceDirectory = Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location));

                CopyFileToTargetProcessDirectory(targetDirectory, sourceDirectory, "Logify.Alert.Core.dll");
                CopyFileToTargetProcessDirectory(targetDirectory, sourceDirectory, "Logify.Alert.Win.dll");
                CopyFileToTargetProcessDirectory(targetDirectory, sourceDirectory, "Logify.Alert.Wpf.dll");
            }
            catch {
            }
        }
        static void CopyFileToTargetProcessDirectory(string targetDirectory, string sourceDirectory, string fileName) {
            string sourceFileName = Path.Combine(sourceDirectory, fileName);
            try {
                if (!File.Exists(sourceFileName)) {
                    Logger.ErrorFormat("File not found: {0}", sourceFileName);
                }
                string targetFileName = Path.Combine(targetDirectory, fileName);
                if (File.Exists(targetFileName))
                    File.Delete(targetFileName);
                if (!File.Exists(targetFileName))
                    File.Copy(sourceFileName, targetFileName);
            }
            catch (Exception ex) {
                Logger.ErrorFormat("Unable to copy file {0} into {1}: {2}", sourceFileName, targetDirectory, ex.ToString());
            }
        }

        static string CreateTargetProcessArguments(string[] args, int from) {
            StringBuilder arguments = new StringBuilder();
            for (int i = from; i < args.Length; i++) {
                arguments.Append(args[i]);
                arguments.Append(' ');
            }
            return arguments.ToString();
        }
        static Process AttachToTargetProcess(RunnerParameters parameters) {
            try {
                if (!String.IsNullOrEmpty(parameters.TargetProcessCommandLine))
                    return StartTargetProcess(parameters.TargetProcessCommandLine, parameters.TargetProcessArgs);
                else if (parameters.Pid != 0) {
                    Process.EnterDebugMode();
                    return Process.GetProcessById(parameters.Pid);
                }
                else
                    return null;
            }
            catch {
                return null;
            }
        }
        static Process StartTargetProcess(string executableFileName, string arguments) {
            Logger.InfoFormat("Starting target process: {0}", executableFileName);
            Process process = new Process();
            process.StartInfo.FileName = executableFileName;
            process.StartInfo.Arguments = arguments;
            if (!process.Start()) {
                Logger.ErrorFormat("FAILED");
                return null;
            }
            Logger.Info("SUCCESS");
            return process;
        }
        static bool IsCompatibleProcess(Process process) {
            if (!Environment.Is64BitOperatingSystem) {
                Logger.Info("Detected 32bit OS");
                return true;
            }
            Logger.Info("Detected 64bit OS");
            Logger.InfoFormat("Loader is running {0}bit platform", Environment.Is64BitProcess ? "64" : "32" );

            bool is64bitProcess = Win32.Is64BitProcess(process);
            Logger.InfoFormat("Target is running {0}bit platform", is64bitProcess ? "64" : "32");

            bool result = Environment.Is64BitProcess == is64bitProcess;
            if (!result) {
                Logger.Error("Target and loader platform doesn't match.");
                Logger.ErrorFormat("Target: {0}bit", is64bitProcess ? "64" : "32");
                Logger.ErrorFormat("Loader: {0}bit", Environment.Is64BitProcess ? "64" : "32");
                Logger.ErrorFormat("Please use {0} loader", is64bitProcess ? "64" : "32");
            }
            else {
                Logger.Info("Target process and loader platform match.");
            }
            return result;
        }

        static bool InjectLogifyAssembly(Process process, int dwProcessId, bool winForms) {
            string exeFileName = Assembly.GetExecutingAssembly().Location;
            string path = Path.GetDirectoryName(Path.GetFullPath(exeFileName));

            ManagedAssemblyInjector injector = new ManagedAssemblyInjector();
            injector.BootstrapDllPath = Path.Combine(path, "InjectManagedAssembly.dll");

            if (!CheckLogifyConfigExists(process))
                return false;

            injector.ManagedAssemblyPath = Path.Combine(path, "Logify.Alert.Inject.dll");
            injector.ManagedClassName = @"DevExpress.Logify.Core.Internal.LogifyInit";
            injector.ManagedMethodName = winForms ? @"RunWinForms" : @"RunWpf";
            injector.ManagedMethodParameter = @"";

            return injector.Inject(process, dwProcessId);
        }
        static bool CheckLogifyConfigExists(Process process) {
            string[] paths = new string[] {
                Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location)),
                Path.GetDirectoryName(process.MainModule.FileName)
            };
            int count = paths.Length;
            for (int i = 0; i < count; i++) {
                if (CheckLogifyConfigExistsCore(paths[i]))
                    return true;
            }
            Logger.ErrorFormat("Required file not found: {0}", "logify.config");
            Logger.Error("Searched locations:");
            for (int i = 0; i < count; i++) {
                Logger.Error(paths[i]);
            }

            return false;
        }
        static bool CheckLogifyConfigExistsCore(string path) {
            string logifyConfigPath = Path.Combine(path, "logify.config");
            return File.Exists(logifyConfigPath);
        }
        static RunnerParameters AnalyzeCommandLineSwitches(Dictionary<string, CommandLineSwitch> switches, string[] args) {
            RunnerParameters result = new RunnerParameters();

            if (!switches.ContainsKey("exec") && !switches.ContainsKey("pid")) {
                Logger.ErrorFormat("Please specify target process ID or command line");
                return null;
            }

            if (switches.ContainsKey("exec")) {
                CommandLineSwitch @switch = switches["exec"];
                result.TargetProcessCommandLine = @switch.Values[0];
                result.TargetProcessArgs = CreateTargetProcessArguments(args, @switch.Index + 1);
            }
            if (switches.ContainsKey("pid")) {
                CommandLineSwitch @switch = switches["pid"];
                string pidText = @switch.Value;
                int pid = TryParsePid(pidText);
                if (pid == 0) {
                    Logger.ErrorFormat("Unable to parse target process Id: {0}", pidText);
                    return null;
                }
                result.Pid = pid;
            }
            result.IsWinforms = switches.ContainsKey("win");
            result.IsWpf = switches.ContainsKey("wpf");
            if (result.IsWinforms == result.IsWpf) {
                Logger.ErrorFormat("Please specify target process type: --win or --wpf");
                return null;
            }
            return result;
        }

        static int TryParsePid(string value) {
            const int invalidPid = 0;
            if (String.IsNullOrEmpty(value))
                return invalidPid;

            int result;
            if (Int32.TryParse(value, out result))
                return result;
            if (value.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                value = value.Substring(2);
            else if (value.StartsWith("x", StringComparison.InvariantCultureIgnoreCase))
                value = value.Substring(1);
            if (Int32.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
                return result;

            return invalidPid;
        }

        static CommandLineConfiguration CreateCommandLineConfiguration() {
            CommandLineConfiguration result = new CommandLineConfiguration();
            result.Add(new CommandLineSwitch() {
                Name = "win",
                Description = "Target process is WinForms application"
            });
            result.Add(new CommandLineSwitch() {
                Name = "wpf",
                Description = "Target process is WPF application"
            });
            result.Add(new CommandLineSwitch() {
                Name = "pid",
                ExpectValue = true,
                Description = "Target process ID. Runner will be attached to process with specified ID."
            });
            result.Add(new CommandLineSwitch() {
                Name = "exec",
                IsDefaultOption = true,
                IsMultiple = true,
                ExpectValue = true,
                Description = "Target process command line"
            });
            return result;
        }
        static void PrintHello() {
            Console.WriteLine();
            Console.WriteLine(String.Format("{0} (C) 2017-{1} DevExpress Inc.", Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), DateTime.Now.Year));
            Console.WriteLine();
        }
        static void PrintUsage(CommandLineConfiguration config) {
            string thisFileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("Usage:");
            Console.WriteLine();
            Console.WriteLine(new CommandLineParser().GetUsage(thisFileName, config));
            Console.WriteLine();
            Console.WriteLine(new CommandLineParser().GetParametersDescription(config));
            Console.WriteLine();
            Console.WriteLine("NOTE: logify.config file should be placed to the same directory where the target process executable or " + thisFileName + " is located.");
            Console.WriteLine("Read more about config file format at: https://github.com/DevExpress/Logify.Alert.Clients/tree/develop/dotnet/Logify.Alert.Win#configuration");
        }
    }

    class RunnerParameters {
        public bool IsWinforms { get; set; }
        public bool IsWpf { get; set; }

        public int Pid { get; set; }
        public string TargetProcessCommandLine { get; set; }
        public string TargetProcessArgs { get; set; }

        public string Error { get; set; }
    }
}
