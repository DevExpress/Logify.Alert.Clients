using System;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Devices.Sensors;
using Windows.System.Profile;

namespace DevExpress.Logify.Core.Internal {
    public class UWPDeviceInfoCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("device");
            try {
                EasClientDeviceInformation eas = new EasClientDeviceInformation();
                logger.WriteValue("manufacturer", eas.SystemManufacturer);
                logger.WriteValue("model", eas.SystemProductName);
                logger.WriteValue("orientation", GetOrientation());
                logger.WriteValue("family", GetDeviceFamily());
            }
            finally {
                logger.EndWriteObject("device");
            }
        }

        private string GetDeviceFamily() {
            try {
                AnalyticsVersionInfo versionInfo = AnalyticsInfo.VersionInfo;
                return versionInfo.DeviceFamily;
            }
            catch {
                return "Unknown";
            }
        }

        private string GetOrientation() {
            try {
                return Windows.Graphics.Display.DisplayInformation.GetForCurrentView().CurrentOrientation.ToString();
            }
            catch {
                return "Unknown";
            }
        }
    }
}
