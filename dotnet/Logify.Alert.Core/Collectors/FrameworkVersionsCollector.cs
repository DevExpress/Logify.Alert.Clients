using Microsoft.Win32;
using System;
using System.Runtime.ExceptionServices;

namespace DevExpress.Logify.Core
{
    public class FrameworkVersionsCollector : IInfoCollector
    {
        [HandleProcessCorruptedStateExceptions]
        public void Process(Exception ex, ILogger logger)
        {
            logger.BeginWriteArray("installedFrameworks");
            try
            {
                using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                {
                    foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                    {
                        if (!versionKeyName.StartsWith("v"))
                            continue;

                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("Version", "");
                        if (!string.IsNullOrEmpty(name))
                        {
                            CollectFrameworkVersion(versionKey, string.Empty, logger);
                        }
                        else
                        {
                            foreach (string profileName in versionKey.GetSubKeyNames())
                            {
                                RegistryKey versionSubKey = versionKey.OpenSubKey(profileName);
                                CollectFrameworkVersion(versionSubKey, profileName, logger);
                            }
                        }
                    }
                }
            }
            finally
            {
                logger.EndWriteArray("installedFrameworks");
            }
        }


        void CollectFrameworkVersion(RegistryKey versionKey, string profile, ILogger logger)
        {
            int result;
            if (!int.TryParse(versionKey.GetValue("Install", "").ToString(), out result) || result != 1)
                return;

            string version = versionKey.GetValue("Version", "").ToString();

            string servicePack = string.Empty;
            if (!string.IsNullOrEmpty(version))
                servicePack = versionKey.GetValue("SP", "").ToString();

            string release = versionKey.GetValue("Release", "").ToString();
            SaveToLog(logger, version, servicePack, profile, release);
        }

        void SaveToLog(ILogger logger, string version, string sp, string profile, string release)
        {
            if (string.IsNullOrEmpty(version) && string.IsNullOrEmpty(release))
                return;

            logger.BeginWriteObject(string.Empty);
            
            WriteValueIfNotEmpty(logger, "version", version);
            WriteValueIfNotEmpty(logger, "servicePack", sp);
            WriteValueIfNotEmpty(logger, "profile", profile);
            WriteValueIfNotEmpty(logger, "release", release);

            logger.EndWriteObject(string.Empty);
        }
        void WriteValueIfNotEmpty(ILogger logger, string key, string value) {
            if (!string.IsNullOrEmpty(value))
                logger.WriteValue(key, value);
        }
    }
}
