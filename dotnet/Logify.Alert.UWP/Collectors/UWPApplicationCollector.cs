using System;
using DevExpress.Logify.Core;
using System.Reflection;
using Windows.ApplicationModel;

namespace DevExpress.Logify.Core.Internal {
    class UWPApplicationCollector : ApplicationCollector {
        public override string AppName {
            get {
                try {
                    return Package.Current.DisplayName;
                }
                catch {
                    return String.Empty;
                }
            }
        }
        public override string AppVersion {
            get {
                try {
                    PackageVersion version = Package.Current.Id.Version;
                    return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
                }
                catch {
                    return String.Empty;
                }
            }
        }

        public override string UserId { get { return String.Empty; } }
        

        public UWPApplicationCollector() : base() {}
    }
}