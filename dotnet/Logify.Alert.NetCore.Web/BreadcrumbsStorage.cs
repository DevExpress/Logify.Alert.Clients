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
}
