using System;
using DevExpress.Logify.Core;
using System.Reflection;
using Android.Content.PM;
using Android.App;
using Android.Content;

namespace DevExpress.Logify.Core.Internal {
    class XamarinApplicationCollector : ApplicationCollector {
        public override string AppName {
            get {
                try {
                    ApplicationInfo appInfo = Application.Context.PackageManager.GetApplicationInfo(Application.Context.PackageName, 0);
                    string appName = Application.Context.PackageManager.GetApplicationLabelFormatted(appInfo).ToString();

                    return appName;
                }
                catch {
                    return String.Empty;
                }
            }
        }
        public override string AppVersion {
            get {
                try {
                    PackageInfo pkgInfo = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0);
                    string appVersion = pkgInfo.VersionName;

                    return appVersion;
                }
                catch {
                    return String.Empty;
                }
            }
        }

        public override string UserId { get { return String.Empty; } }
        

        public XamarinApplicationCollector() : base() {}
    }
}