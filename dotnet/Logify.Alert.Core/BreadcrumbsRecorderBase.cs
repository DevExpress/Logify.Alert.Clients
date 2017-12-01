using System;

namespace DevExpress.Logify.Core.Internal {
    public abstract class BreadcrumbsRecorderBase {
        [IgnoreCallTracking]
        protected void AddBreadcrumb(Breadcrumb item) {
            if(LogifyClientBase.Instance != null) {
                PopulateCommonBreadcrumbInfo(item);
                LogifyClientBase.Instance.Breadcrumbs.AddSimple(item);
            }
        }
        [IgnoreCallTracking]
        void PopulateCommonBreadcrumbInfo(Breadcrumb item) {
            item.DateTime = DateTime.Now;
            item.Level = BreadcrumbLevel.Info;
            item.ThreadId = GetThreadId();
            item.Category = GetCategory();
            item.IsAuto = true;
        }
        protected virtual string GetCategory() {
            return "input";
        }
        protected abstract string GetThreadId();
    }
}
