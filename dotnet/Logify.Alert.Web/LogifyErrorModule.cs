using System;
using DevExpress.Logify.Core;
using System.Web.Configuration;

namespace DevExpress.Logify.Web {
    public static class Register {
        public static void Exception(Exception e) {
            LogifyAlert.Instance.Send(e);
        }
    }
}