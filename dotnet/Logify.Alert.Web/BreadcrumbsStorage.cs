using System.Web;
using DevExpress.Logify.Core;

namespace DevExpress.Logify.Web {
    public interface IBreadcrumbsStorage {
        BreadcrumbCollection Breadcrumbs { get; }
    }
    public class InMemoryBreadcrumbsStorage : IBreadcrumbsStorage {
        readonly BreadcrumbCollection _breadcrumbs;
        public BreadcrumbCollection Breadcrumbs {
            get {
                return this._breadcrumbs;
            }
        }
        public InMemoryBreadcrumbsStorage() {
            this._breadcrumbs = new BreadcrumbCollection(50);
        }
    }
    public class HttpBreadcrumbsStorage : IBreadcrumbsStorage {
        const string SessionKey = "Logify.Web.Breadcrumbs";
        public BreadcrumbCollection Breadcrumbs {
            get {
                if(HttpContext.Current.Session[SessionKey] == null)
                    HttpContext.Current.Session[SessionKey] = new BreadcrumbCollection(50);
                return (BreadcrumbCollection)HttpContext.Current.Session[SessionKey];
            }
        }
    }
}
