using DevExpress.Logify.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DevExpress.Logify.WPF {
    public class WPFBreadcrumbsRecorder: BreadcrumbsRecorderBase {
        static WPFBreadcrumbsRecorder instance = null;
        public static WPFBreadcrumbsRecorder Instance {
            get {
                if(instance == null) {
                    instance = new WPFBreadcrumbsRecorder();
                }
                return instance;
            }
        }

        static volatile bool initialized;
        static readonly object Locker = new object();
        bool IsActive = false;
        PreviousArgs previousArgs = null;

        private WPFBreadcrumbsRecorder() { }

        public void BeginCollect() {
            IsActive = true;
            FocusObserver.IsActive = true;
            Initialize();
        }
        void Initialize() {
            if(initialized)
                return;
            lock(Locker) {
                if(initialized)
                    return;
                try {
                    FocusObserver.FocusChanged += FocusObserverOnFocusChanged;
                    SubscribeToControl();
                } finally {
                    initialized = true;
                }
            }
        }
        public void EndCollect() {
            IsActive = false;
            FocusObserver.IsActive = false;
        }
        protected override string GetThreadId() {
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }
        void SubscribeToControl() {
            Subscribe(typeof(Control));
        }
        void Subscribe(Type type) {
            EventManager.RegisterClassHandler(type, UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(MouseDown), true);
            EventManager.RegisterClassHandler(type, UIElement.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseUp), true);
            EventManager.RegisterClassHandler(type, UIElement.PreviewKeyDownEvent, new KeyEventHandler(KeyDown), true);
            EventManager.RegisterClassHandler(type, UIElement.PreviewKeyUpEvent, new KeyEventHandler(KeyUp), true);
            EventManager.RegisterClassHandler(type, UIElement.PreviewTextInputEvent, new TextCompositionEventHandler(TextInput), true);
            //EventManager.RegisterClassHandler(type, UIElement.PreviewMouseWheelEvent, new MouseWheelEventHandler(MouseWheel), true);
        }
        void FocusObserverOnFocusChanged(object sender, ValueChangedEventArgs<IInputElement> e) {
            FrameworkElement oldFocus = e.OldValue as FrameworkElement;
            if(oldFocus != null) {
                Dictionary<string, string> properties = CollectCommonProperties(oldFocus, null);
                LogFocus(properties, false);
            }
            FrameworkElement newFocus = e.NewValue as FrameworkElement;
            if(newFocus != null) {
                Dictionary<string, string> properties = CollectCommonProperties(newFocus, null);
                LogFocus(properties, true);
            }
        }
        void KeyDown(object sender, KeyEventArgs e) {
            FrameworkElement source = sender as FrameworkElement;
            if(!IsActive || (sender == null))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e);
            LogKeyboard(properties, e, false, CheckPasswordElement(e.OriginalSource as UIElement));
        }
        void KeyUp(object sender, KeyEventArgs e) {
            FrameworkElement source = sender as FrameworkElement;
            if(!IsActive || (sender == null))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e);
            LogKeyboard(properties, e, true, CheckPasswordElement(e.OriginalSource as UIElement));
        }
        void MouseDown(object sender, MouseButtonEventArgs e) {
            FrameworkElement source = sender as FrameworkElement;
            if(!IsActive || (sender == null))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e);
            LogMouse(properties, source, e, false);
        }
        void MouseUp(object sender, MouseButtonEventArgs e) {
            FrameworkElement source = sender as FrameworkElement;
            if(!IsActive || (sender == null))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e);
            LogMouse(properties, source, e, true);
        }
        void MouseWheel(object sender, MouseWheelEventArgs e) {
            FrameworkElement source = sender as FrameworkElement;
            if(!IsActive || (sender == null))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e);
            LogMouseWheel(properties, e);
        }
        void TextInput(object sender, TextCompositionEventArgs e) {
            FrameworkElement source = sender as FrameworkElement;
            if(!IsActive || (sender == null))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e);
            LogTextInput(properties, e, CheckPasswordElement(e.OriginalSource as UIElement));
        }
        Dictionary<string, string> CollectCommonProperties(FrameworkElement source, EventArgs e) {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties["Name"] = source.Name;
            properties["ClassName"] = source.GetType().ToString();

            if(previousArgs == null) {
                previousArgs = new PreviousArgs() { EventArgs = e, Guid = Guid.NewGuid().ToString() };
            } else {
                if(e == null || !Object.Equals(previousArgs.EventArgs, e)) {
                    previousArgs = new PreviousArgs() { EventArgs = e, Guid = Guid.NewGuid().ToString() };
                }
            }
            properties["#e"] = previousArgs.Guid;

            AutomationPeer automation = GetAutomationPeer(source);
            if(automation == null)
                return properties;

            properties["Name"] = string.IsNullOrEmpty(source.Name) ? automation.GetName() : source.Name;

            string itemType = automation.GetItemType();
            if(!string.IsNullOrEmpty(itemType))
                properties["ItemType"] = itemType;

            var automationId = automation.GetAutomationId();
            if(!string.IsNullOrEmpty(automationId))
                properties["AutomationID"] = automationId;

            properties["ControlType"] = automation.GetAutomationControlType().ToString();
            properties["windowCaption"] = properties["Name"];

            CollectValue(properties, automation);
            return properties;
        }
        AutomationPeer GetAutomationPeer(UIElement source) {
            return UIElementAutomationPeer.CreatePeerForElement(source);
        }
        void CollectValue(Dictionary<string, string> properties, AutomationPeer automation) {
            if(automation.IsPassword()) {
                properties["Value"] = "*";
            } else {
                IValueProvider valueProvider = automation.GetPattern(PatternInterface.Value) as IValueProvider;
                if(valueProvider != null) {
                    properties["Value"] = valueProvider.Value;
                }
            }
        }
        void LogMouse(IDictionary<string, string> properties, FrameworkElement source, MouseButtonEventArgs e, bool isUp) {
            properties["ButtonState"] = e.ButtonState.ToString();
            properties["mouseButton"] = e.ChangedButton.ToString();
            properties["ClickCount"] = e.ClickCount.ToString();
            Breadcrumb item = new Breadcrumb();
            if(e.ClickCount == 2) {
                properties["action"] = "doubleClick";
                item.Event = BreadcrumbEvent.MouseDoubleClick;
            } else if(isUp) {
                properties["action"] = "up";
                item.Event = BreadcrumbEvent.MouseUp;
            } else {
                properties["action"] = "down";
                item.Event = BreadcrumbEvent.MouseDown;
            }
            CollectMousePosition(properties, source, e);
            item.CustomData = properties;

            AddBreadcrumb(item);
        }
        void CollectMousePosition(IDictionary<string, string> properties, FrameworkElement source, MouseButtonEventArgs e) {
            IInputElement inputElement = GetRootInputElement(source);
            if(inputElement != null) {
                Point relativePosition = e.GetPosition(inputElement);
                properties["x"] = relativePosition.X.ToString();
                properties["y"] = relativePosition.Y.ToString();
                if(inputElement is Visual) {
                    Point screenPosition = (inputElement as Visual).PointToScreen(relativePosition);
                    properties["sx"] = screenPosition.X.ToString();
                    properties["sy"] = screenPosition.Y.ToString();
                }
            }
        }
        IInputElement GetRootInputElement(FrameworkElement source) {
            return GetRootInputElementCore(source, null);
        }
        IInputElement GetRootInputElementCore(FrameworkElement source, IInputElement lastInputElement) {
            if(source is IInputElement)
                lastInputElement = source as IInputElement;

            if(source != null) {
                return GetRootInputElementCore(source.Parent as FrameworkElement, lastInputElement);
            }
            return lastInputElement;
        }
        void LogMouseWheel(IDictionary<string, string> properties, MouseWheelEventArgs e) {
            properties["Delta"] = e.Delta.ToString();
            properties["LeftButton"] = e.LeftButton.ToString();
            properties["MiddleButton"] = e.MiddleButton.ToString();
            properties["RightButton"] = e.RightButton.ToString();

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.MouseWheel;
            item.CustomData = properties;

            AddBreadcrumb(item);
        }
        void LogTextInput(IDictionary<string, string> properties, TextCompositionEventArgs e, bool isPassword) {
            properties["Text"] = isPassword ? "*" : e.Text;
            properties["char"] = isPassword ? "*" : e.Text;
            properties["SystemText"] = isPassword ? "*" : e.SystemText;
            properties["action"] = "press";

            Breadcrumb item = new Breadcrumb();
            item.Event = BreadcrumbEvent.KeyPress;
            item.CustomData = properties;

            AddBreadcrumb(item);
        }
        void LogFocus(IDictionary<string, string> properties, bool isGotFocus) {
            Breadcrumb item = new Breadcrumb();
            item.Event = isGotFocus ? BreadcrumbEvent.GotFocus : BreadcrumbEvent.LostFocus;
            item.CustomData = properties;

            AddBreadcrumb(item);
        }
        void LogKeyboard(IDictionary<string, string> properties, KeyEventArgs e, bool isUp, bool isPassword) {
            properties["key"] = GetKeyValue(e.Key, isPassword).ToString();
            properties["SystemKey"] = GetKeyValue(e.SystemKey, isPassword).ToString();
            properties["IsToggled"] = e.IsToggled.ToString();
            properties["IsRepeat"] = e.IsRepeat.ToString();
            properties["KeyStates"] = e.KeyStates.ToString();
            properties["action"] = isUp ? "up" : "down";
            properties["scanCode"] = KeyInterop.VirtualKeyFromKey(GetKeyValue(e.Key, isPassword)).ToString();

            Breadcrumb item = new Breadcrumb();
            item.Event = isUp ? BreadcrumbEvent.KeyUp : BreadcrumbEvent.KeyDown;
            item.CustomData = properties;

            AddBreadcrumb(item);
        }
        Key GetKeyValue(Key key, bool isPassword) {
            if(!isPassword)
                return key;

            switch(key) {
                case Key.Tab:
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.PageUp:
                case Key.PageDown:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.Enter:
                case Key.Home:
                case Key.End:
                    return key;

                default:
                    return Key.Multiply;
            }
        }
        bool CheckPasswordElement(UIElement targetElement) {
            if(targetElement != null) {
                AutomationPeer automationPeer = GetAutomationPeer(targetElement);
                return (automationPeer != null) ? automationPeer.IsPassword() : false;
            }
            return false;
        }
    }
    static class LinqExtensions {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            if(enumerable == null)
                return Enumerable.Empty<T>();
            foreach(var t in enumerable)
                action(t);
            return enumerable;
        }
    }
    class ValueChangedEventArgs<T> : EventArgs {
        private T _NewValue;
        private T _OldValue;

        public ValueChangedEventArgs(T oldValue, T newValue) {
            _OldValue = oldValue;
            _NewValue = newValue;
        }

        public T NewValue { get { return _NewValue; } }
        public T OldValue { get { return _OldValue; } }
    }
    static class FocusObserver {
        static readonly object lock_ = new object();
        [ThreadStatic]
        static WeakReference oldWR;
        [ThreadStatic]
        static WeakReference currentWR;

        internal static bool IsActive = false;

        public static IInputElement CurrentFocus {
            get { return (IInputElement)currentWR.With(x => x.Target); }
            private set { currentWR = new WeakReference(value); }
        }
        public static IInputElement PreviousFocus {
            get { return (IInputElement)oldWR.With(x => x.Target); }
            private set { oldWR = new WeakReference(value); }
        }
        public static TR With<TI, TR>(this TI input, Func<TI, TR> evaluator)
            where TI : class
            where TR : class {
            if(input == null)
                return null;
            return evaluator(input);
        }
        static volatile bool isInitialized = false;
        static void Initialize() {
            if(isInitialized)
                return;
            lock(lock_) {
                if(isInitialized)
                    return;
                isInitialized = true;
                CurrentFocus = Keyboard.FocusedElement;
                EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnKeyboardFocusChanged), true);
                EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnKeyboardFocusChanged), true);
            }
        }
        static FocusObserver() {
            Initialize();
        }
        static void OnKeyboardFocusChanged(object sender, KeyboardFocusChangedEventArgs e) {
            if(!IsActive)
                return;

            if(e.OldFocus == CurrentFocus)
                AddRecord(e.NewFocus);
            else if(CurrentFocus != Keyboard.FocusedElement)
                AddRecord(Keyboard.FocusedElement);
        }

        public delegate void ValueChangedEventHandler<T>(object sender, ValueChangedEventArgs<T> e);

        public static event ValueChangedEventHandler<IInputElement> FocusChanged;
        static void RaiseFocusChanged(IInputElement oldElement, IInputElement newElement) {
            FocusChanged?.Invoke(null, new ValueChangedEventArgs<IInputElement>(oldElement, newElement));
        }
        static void AddRecord(IInputElement focusedElement) {
            PreviousFocus = CurrentFocus;
            CurrentFocus = focusedElement;
            if(PreviousFocus != CurrentFocus)
                RaiseFocusChanged(PreviousFocus, CurrentFocus);
        }
    }
    class PreviousArgs {
        public EventArgs EventArgs { get; set; }
        public string Guid { get; set; }
    }
}
