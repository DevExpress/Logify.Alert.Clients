using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            item.Category = "input";
            item.IsAuto = true;
        }
        protected abstract string GetThreadId();
    }
}
