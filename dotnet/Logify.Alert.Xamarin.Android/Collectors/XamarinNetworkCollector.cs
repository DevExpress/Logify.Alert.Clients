using Android;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Runtime;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinNetworkCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            if (Application.Context.PackageManager.CheckPermission(Manifest.Permission.AccessNetworkState, Application.Context.PackageName) == Android.Content.PM.Permission.Denied) {
                return;
            }
            logger.BeginWriteObject("network");
            try {
                ConnectivityManager connectivityManager = Application.Context.GetSystemService(Activity.ConnectivityService).JavaCast<ConnectivityManager>();
                logger.WriteValue("typeName", GetNetworkTypeName(connectivityManager));
            }
            finally {
                logger.EndWriteObject("network");
            }
        }

        string GetNetworkTypeName(ConnectivityManager manager) {
            try {
                string name = manager.ActiveNetworkInfo.TypeName;
                string subName = manager.ActiveNetworkInfo.SubtypeName;

                if (string.IsNullOrEmpty(subName)) {
                    if (string.IsNullOrEmpty(name)) {
                        return string.Empty;
                    }
                    return name;
                }
                return string.Format("{0} ({1})", name, subName);
            }
            catch {
                return string.Empty;
            }
        }
    }
}
