using System;
using DevExpress.Logify.Core;
using System.Reflection;
using Foundation;

namespace DevExpress.Logify.Core.Internal {
    class XamarinApplicationCollector : ApplicationCollector {
        public override string AppName {
            get {
                try {
                    string appName = NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleName")).ToString();
                    return appName;
                }
                catch {
                    return String.Empty;
                }
            }
        }
        public override string AppVersion {
            get {
                string appVersion;
                try {
                    appVersion = NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleShortVersionString")).ToString();
                }
                catch {
                    return String.Empty;
                }
                try {
                    string buildNumber = NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleVersion")).ToString();
                    return string.Format("{0}.{1}", appVersion, buildNumber);
                }
                catch {
                    return appVersion;
                }
            }
        }

        public override string UserId { get { return String.Empty; } }
        

        public XamarinApplicationCollector() : base() {}
    }
}