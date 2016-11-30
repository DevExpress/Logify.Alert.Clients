using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using DevExpress.Utils.Zip;
namespace DevExpress.Logify.Send {
    public class AutoUpdater {
        static readonly string[] knownFileNames = new string[] {
            "LogifySend.exe",
            "Logify.Client.Core.dll",
            "Logify.Client.Win.dll"
        };

        public void Run(string apiKey) {
            string workingFolder = DetectWorkingFolder();
            if (String.IsNullOrEmpty(workingFolder))
                return;

            if (!CanCreateFilesInWorkingFolder(workingFolder))
                return;

            string backupFolder = DetectBackupFolder(workingFolder);
            if (!CleanBackupFolder(backupFolder))
                return;

            string checksumFileName = TryDownloadUpgradeChecksum(backupFolder, apiKey);
            if (String.IsNullOrEmpty(checksumFileName))
                return;

            string upgradeFileName = TryDownloadUpgrade(backupFolder, apiKey);
            if (String.IsNullOrEmpty(upgradeFileName))
                return;

            if (!ValidateUpgradeArchive(upgradeFileName, checksumFileName))
                return;
            
            if (!MoveKnownFilesToBackupFolder(workingFolder, backupFolder))
                return;

            UnpackUpgradeToWorkingFolder(upgradeFileName, workingFolder);
        }

        bool ValidateUpgradeArchive(string upgradeFileName, string checksumFileName) {
            try {
                byte[] expectedChecksum = File.ReadAllBytes(checksumFileName);
                if (expectedChecksum == null || expectedChecksum.Length != 16)
                    return false;
                byte[] checksum;
                using (FileStream stream = new FileStream(upgradeFileName, FileMode.Open)) {
                    using (MD5 md5 = MD5.Create()) {
                        checksum = md5.ComputeHash(stream);
                    }
                }
                if (checksum == null || checksum.Length != 16)
                    return false;

                for (int i = 0; i < checksum.Length; i++)
                    if (checksum[i] != expectedChecksum[i])
                        return false;

                return true;
            }
            catch {
                return false;
            }
        }


        string DetectWorkingFolder() {
            return Path.GetDirectoryName(GetType().Assembly.Location);
        }

        bool CanCreateFilesInWorkingFolder(string workingFolder) {
            try {
                string fileName = Path.GetTempFileName();
                fileName = Path.Combine(workingFolder, fileName);
                File.WriteAllText(fileName, "test");
                if (!File.Exists(fileName))
                    return false;
                File.Delete(fileName);
                return true;
            }
            catch {
                return false;
            }
        }
        string DetectBackupFolder(string workingFolder) {
            string backupFolder = Path.Combine(workingFolder, "Temp");
            if (Directory.Exists(backupFolder))
                return backupFolder;
            Directory.CreateDirectory(backupFolder);
            if (!Directory.Exists(backupFolder))
                return String.Empty;
            return backupFolder;
        }
        bool CleanBackupFolder(string backupFolder) {
            bool unableToDeleteKnownFiles = false;
            string[] files = Directory.GetFiles(backupFolder);
            foreach (string fileName in files) {
                try {
                    File.Delete(fileName);
                }
                catch {
                    if (IsKnownFileName(fileName))
                        unableToDeleteKnownFiles = true;
                }
            }
            return !unableToDeleteKnownFiles;
        }
        bool IsKnownFileName(string fileName) {
            foreach (string knownFileName in knownFileNames) {
                if (String.Compare(Path.GetFileName(fileName), knownFileName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return true;
            }
            return false;
        }
        bool MoveKnownFilesToBackupFolder(string workingFolder, string backupFolder) {
            List<string> movedFiles = new List<string>();
            try {
                foreach (string knownFileName in knownFileNames) {
                    string fileName = Path.Combine(workingFolder, knownFileName);
                    if (File.Exists(fileName)) {
                        File.Move(fileName, Path.Combine(backupFolder, knownFileName));
                        movedFiles.Add(knownFileName);
                    }
                }

                return true;
            }
            catch {
                // move back any moved files
                try {
                    foreach (string fileName in movedFiles) {
                        File.Move(Path.Combine(backupFolder, fileName), Path.Combine(workingFolder, fileName));
                    }
                }
                catch {
                }
                return false;
            }
        }

        //const string serviceUrl = "http://logify.devexpress.com:8080";
        const string serviceUrl = "https://logify.devexpress.com/api/upgrade";
        //const string serviceUrl = "http://localhost:53179";

        string TryDownloadUpgradeChecksum(string backupFolder, string apiKey) {
            
            using (HttpClient client = new HttpClient()) {
                string url = serviceUrl + "/GetClientUpgrade?apiKey=" + apiKey + "&version=" + GetType().Assembly.GetName().Version.ToString() + "&checkSum=true";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", apiKey);
                HttpResponseMessage message = client.GetAsync(url).Result;
                if (message == null || message.StatusCode != HttpStatusCode.OK)
                    return String.Empty;
                byte[] bytes = message.Content.ReadAsByteArrayAsync().Result;
                string fileName = Path.Combine(backupFolder, "checksum.bin");
                File.WriteAllBytes(fileName, bytes);
                return fileName;
            }
        }
        string TryDownloadUpgrade(string backupFolder, string apiKey) {
            using (HttpClient client = new HttpClient()) {
                string url = serviceUrl + "/GetClientUpgrade?apiKey=" + apiKey + "&version=" + GetType().Assembly.GetName().Version.ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", apiKey);
                HttpResponseMessage message = client.GetAsync(url).Result;
                if (message == null || message.StatusCode != HttpStatusCode.OK)
                    return String.Empty;
                byte[] bytes = message.Content.ReadAsByteArrayAsync().Result;
                string fileName = Path.Combine(backupFolder, "upgrade.bin");
                File.WriteAllBytes(fileName, bytes);
                return fileName;
            }
            //return @"C:\Projects\2015.2\Logify\Logify_14_35.zip";
        }
        void UnpackUpgradeToWorkingFolder(string upgradeFileName, string workingFolder) {
            using (FileStream stream = new FileStream(upgradeFileName, FileMode.Open)) {
                InternalZipFileCollection files = InternalZipArchive.Open(stream);
                foreach (InternalZipFile file in files) {
                    SaveFile(Path.Combine(workingFolder, Path.GetFileName(file.FileName)), file);
                }
            }
        }

        void SaveFile(string fileName, InternalZipFile file) {
            using (FileStream stream = new FileStream(fileName, FileMode.Create)) {
                StreamHelper.WriteTo(file.FileDataStream, stream);
            }
            FileInfo fileInfo = new FileInfo(fileName);
            fileInfo.LastWriteTime = file.FileLastModificationTime;
        }
    }
    public static class StreamHelper {
        public static void WriteTo(Stream inputStream, Stream outputStream) {
            byte[] buf = new byte[1024];
            int readedByteCount;
            for (; ; ) {
                readedByteCount = inputStream.Read(buf, 0, buf.Length);
                if (readedByteCount == 0) break;
                outputStream.Write(buf, 0, readedByteCount);
            }
        }
    }
}