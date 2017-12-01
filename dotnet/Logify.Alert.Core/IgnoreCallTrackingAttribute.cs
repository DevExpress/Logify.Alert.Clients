using System;

namespace DevExpress.Logify.Core.Internal {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Constructor)]
    public class IgnoreCallTrackingAttribute : Attribute {
    }
}