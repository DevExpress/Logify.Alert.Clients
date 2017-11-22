using System;
using DevExpress.Logify.Core.Internal;

namespace DevExpress.Logify.Core {
    public abstract class BreadcrumbsRecorderBase {
        protected void AddBreadcrumb(Breadcrumb item) {
            if(LogifyClientBase.Instance != null) {
                PopulateCommonBreadcrumbInfo(item);
                LogifyClientBase.Instance.Breadcrumbs.AddSimple(item);
            }
        }
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
