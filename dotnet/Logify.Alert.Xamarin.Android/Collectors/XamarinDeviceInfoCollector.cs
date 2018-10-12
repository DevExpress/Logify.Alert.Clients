using System;
using Android.Content;
using Android.Runtime;
using Android.Views;

namespace DevExpress.Logify.Core.Internal {
    public class XamarinDeviceInfoCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            logger.BeginWriteObject("device");
            try {
                logger.WriteValue("manufacturer", Android.OS.Build.Manufacturer);
                logger.WriteValue("model", Android.OS.Build.Model);
                logger.WriteValue("orientation", GetOrientation());
                logger.WriteValue("product", Android.OS.Build.Product);
            }
            finally {
                logger.EndWriteObject("device");
            }
        }

        private string GetOrientation() {
            try {
                IWindowManager windowManager = Android.App.Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

                SurfaceOrientation rotation = windowManager.DefaultDisplay.Rotation;
                bool isLandscape = rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270;
                return isLandscape ? "Landscape" : "Portrait";
            }
            catch {
                return "Unknown";
            }
        }
    }
}
