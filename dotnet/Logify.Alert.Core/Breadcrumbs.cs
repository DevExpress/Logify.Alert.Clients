using DevExpress.Logify.Core.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DevExpress.Logify.Core {
    public class BreadcrumbCollection : IEnumerable<Breadcrumb> {
        readonly Breadcrumb[] items;
        int maxSize;
        int spaceLeft;
        int nextIndex = 0;

        public int Count {
            get {
                if (spaceLeft > 0)
                    return maxSize - spaceLeft;
                else
                    return maxSize;
            }
        }
        internal int MaxSize { get { return maxSize; } }

        public BreadcrumbCollection(int maxSize = 1000) {
            if (maxSize < 1)
                throw new ArgumentException();
            this.maxSize = maxSize;
            this.items = new Breadcrumb[maxSize];
            this.spaceLeft = maxSize;
        }

        internal static BreadcrumbCollection ChangeSize(BreadcrumbCollection sourceCollection, int newSize) {
            if (sourceCollection == null || sourceCollection.MaxSize == newSize || newSize <= 1)
                return sourceCollection;

            BreadcrumbCollection result = new BreadcrumbCollection(newSize);
            IEnumerator<Breadcrumb> forward = result.GetEnumerator();
            while (forward.MoveNext())
                result.Add(forward.Current);
            return result;
        }

        [IgnoreCallTracking]
        internal void AddCore(Breadcrumb item) {
            if (item.DateTime == DateTime.MinValue)
                item.DateTime = DateTime.UtcNow;
            items[nextIndex] = item;
            nextIndex = (nextIndex + 1) % maxSize;
            if (spaceLeft > 0)
                spaceLeft--;
        }

        public IEnumerator<Breadcrumb> GetEnumerator() {
            if (spaceLeft > 0) {
                for (int i = nextIndex - 1; i >= 0; i--)
                    yield return items[i];
            }
            else {
                for (int i = maxSize - 1; i >= 0; i--)
                    yield return items[(nextIndex + i) % maxSize];
            }
        }

        internal IEnumerator<Breadcrumb> GetForwardEnumerator() {
            if (spaceLeft > 0) {
                for (int i = 0; i < nextIndex; i++)
                    yield return items[i];
            }
            else {
                for (int i = 0; i < maxSize; i++)
                    yield return items[(nextIndex + i) % maxSize];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

#if ALLOW_ASYNC
        public void Add(Breadcrumb item, [CallerMemberName] string methodName = "", [CallerLineNumber] int line = 0) {
            if (item != null) {
                if (String.IsNullOrEmpty(item.MethodName) && !String.IsNullOrEmpty(methodName))
                    item.MethodName = methodName;
                if (item.Line == 0)
                    item.Line = line;
            }
            AddCore(item);
        }
#else
        public void Add(Breadcrumb item) {
            AddCore(item);
        }
#endif
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

        static readonly BreadcrumbEvent manual = new BreadcrumbEvent("manual");
        public static BreadcrumbEvent Manual { get { return manual; } }

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

        static readonly BreadcrumbEvent focusChange = new BreadcrumbEvent("focusChange");
        public static BreadcrumbEvent FocusChange { get { return focusChange; } }

        static readonly BreadcrumbEvent gotFocus = new BreadcrumbEvent("gotFocus");
        public static BreadcrumbEvent GotFocus { get { return gotFocus; } }

        static readonly BreadcrumbEvent lostFocus = new BreadcrumbEvent("lostFocus");
        public static BreadcrumbEvent LostFocus { get { return lostFocus; } }

        static readonly BreadcrumbEvent request = new BreadcrumbEvent("request");
        public static BreadcrumbEvent Request { get { return request; } }

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
        internal bool IsAuto { get; set; }
    }
}

namespace DevExpress.Logify.Core.Internal {
    public static class BreadcrumbExtensions {
        public static bool GetIsAuto(this Breadcrumb instance) {
            return instance.IsAuto;
        }
    }
    //public void Add(Breadcrumb item, [CallerMember])
    public static class BreadcrumbCollectionExtensions {
        [IgnoreCallTracking]
        public static void AddSimple(this BreadcrumbCollection items, Breadcrumb item) {
            items.AddCore(item);
        }
    }
}