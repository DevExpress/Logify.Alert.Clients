using System;
using DevExpress.Logify.Core;
using System.Web.Configuration;
using System.ComponentModel;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Web {
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the LogifyAlert.Instance.Send instead.")]
    public static class Register {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Use the LogifyAlert.Instance.Send instead.")]
        [IgnoreCallTracking]
        public static void Exception(Exception e) {
            LogifyAlert.Instance.Send(e);
        }
    }
}