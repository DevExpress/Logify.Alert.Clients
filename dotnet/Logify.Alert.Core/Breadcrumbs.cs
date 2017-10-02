using System;
using System.Collections.Generic;

namespace DevExpress.Logify.Core {
    public class BreadcrumbCollection : List<Breadcrumb> {
    }
    public enum BreadcrumbLevel {
        None = 0,
        Debug,
        Info,
        Warning,
        Error,
    }
    public struct BreadcrumbEvent {
        static readonly BreadcrumbEvent none = new BreadcrumbEvent("none");
        public static BreadcrumbEvent None { get { return none; } }

        static readonly BreadcrumbEvent mouseDown = new BreadcrumbEvent("mouseDown");
        public static BreadcrumbEvent MouseDown { get { return mouseDown; } }

        static readonly BreadcrumbEvent mouseUp = new BreadcrumbEvent("mouseUp");
        public static BreadcrumbEvent MouseUp { get { return mouseUp; } }

        static readonly BreadcrumbEvent mouseClick = new BreadcrumbEvent("mouseClick");
        public static BreadcrumbEvent MouseClick { get { return mouseClick; } }

        static readonly BreadcrumbEvent mouseDoubleClick = new BreadcrumbEvent("mouseDoubleClick");
        public static BreadcrumbEvent MouseDoubleClick { get { return mouseDoubleClick; } }

        static readonly BreadcrumbEvent mouseMove = new BreadcrumbEvent("mouseMove");
        public static BreadcrumbEvent MouseMove { get { return mouseMove; } }

        static readonly BreadcrumbEvent mouseWheel = new BreadcrumbEvent("mouseWheel");
        public static BreadcrumbEvent MouseWheel { get { return mouseWheel; } }

        static readonly BreadcrumbEvent keyDown = new BreadcrumbEvent("keyDown");
        public static BreadcrumbEvent KeyDown { get { return keyDown; } }

        static readonly BreadcrumbEvent keyUp = new BreadcrumbEvent("keyUp");
        public static BreadcrumbEvent KeyUp { get { return keyUp; } }

        static readonly BreadcrumbEvent keyPress = new BreadcrumbEvent("keyPress");
        public static BreadcrumbEvent KeyPress { get { return keyPress; } }

        static readonly BreadcrumbEvent typeText = new BreadcrumbEvent("typeText");
        public static BreadcrumbEvent TypeText { get { return typeText; } }

        static readonly BreadcrumbEvent dialogOpen = new BreadcrumbEvent("dialogOpen");
        public static BreadcrumbEvent DialogOpen { get { return dialogOpen; } }

        static readonly BreadcrumbEvent dialogClose = new BreadcrumbEvent("dialogClose");
        public static BreadcrumbEvent DialogClose { get { return dialogClose; } }

        static readonly BreadcrumbEvent windowActivate = new BreadcrumbEvent("windowActivate");
        public static BreadcrumbEvent WindowActivate { get { return windowActivate; } }


        readonly string id;

        public BreadcrumbEvent(string id) {
            this.id = id;
        }

        public override string ToString() {
            return id;
        }

        public override int GetHashCode() {
            return id != null ? id.GetHashCode() : None.GetHashCode();
        }
        public override bool Equals(object obj) {
            if (!(obj is BreadcrumbEvent))
                return false;
            return IsEqual(this, (BreadcrumbEvent)obj);
        }
        static bool IsEqual(BreadcrumbEvent first, BreadcrumbEvent second) {
            if (String.IsNullOrEmpty(second.id) && String.IsNullOrEmpty(first.id))
                return true;
            return Object.Equals(first.id, second.id);
        }

        public static bool operator ==(BreadcrumbEvent first, BreadcrumbEvent second) {
            return IsEqual(first, second);
        }
        public static bool operator !=(BreadcrumbEvent first, BreadcrumbEvent second) {
            return !IsEqual(first, second);
        }
    }
    public class Breadcrumb {
        public Breadcrumb() {
            this.Event = BreadcrumbEvent.None;
        }

        public DateTime DateTime { get; set; }
        public BreadcrumbLevel Level { get; set; }
        public BreadcrumbEvent Event { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public int Line { get; set; }
        public string ThreadId { get; set; }
        public IDictionary<string, string> CustomData { get; set; }
    }
}