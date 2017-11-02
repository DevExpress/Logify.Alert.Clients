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
            Subscribe(typeof(FrameworkElement));
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
            if(e.OldValue is FrameworkElement oldFocus) {
                Dictionary<string, string> properties = CollectCommonProperties(oldFocus, oldFocus);
                LogFocus(properties, false);
            }
            if(e.NewValue is FrameworkElement newFocus) {
                Dictionary<string, string> properties = CollectCommonProperties(newFocus, newFocus);
                LogFocus(properties, true);
            }
        }
        void KeyDown(object sender, KeyEventArgs e) {
            if(!IsActive || !(sender is FrameworkElement source))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e.OriginalSource as FrameworkElement);
            LogKeyboard(properties, e, false, CheckPasswordElement(e.OriginalSource as UIElement));
        }
        void KeyUp(object sender, KeyEventArgs e) {
            if(!IsActive || !(sender is FrameworkElement source))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e.OriginalSource as FrameworkElement);
            LogKeyboard(properties, e, true, CheckPasswordElement(e.OriginalSource as UIElement));
        }
        void MouseDown(object sender, MouseButtonEventArgs e) {
            if(!IsActive || !(sender is FrameworkElement source))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e.OriginalSource as FrameworkElement);
            LogMouse(properties, e, false);
        }
        void MouseUp(object sender, MouseButtonEventArgs e) {
            if(!IsActive || !(sender is FrameworkElement source))
                return;
            Dictionary<string, string> properties = CollectCommonProperties(source, e.OriginalSource as FrameworkElement);
            LogMouse(properties, e, true);
        }
        void MouseWheel(object sender, MouseWheelEventArgs e) {
            if(!IsActive || !(sender is FrameworkElement source))
                return;
            Dictionary<string, string> properties = CollectCommonProperties(source, e.OriginalSource as FrameworkElement);
            LogMouseWheel(properties, e);
        }
        void TextInput(object sender, TextCompositionEventArgs e) {
            if(!IsActive || !(sender is FrameworkElement source))
                return;

            Dictionary<string, string> properties = CollectCommonProperties(source, e.OriginalSource as FrameworkElement);
            LogTextInput(properties, e, CheckPasswordElement(e.OriginalSource as UIElement));
        }
        Dictionary<string, string> CollectCommonProperties(FrameworkElement source, FrameworkElement originalSource) {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties["Name"] = source.Name;
            properties["ClassName"] = source.GetType().ToString();
            if(IsHiddenItem(source, originalSource))
                properties["#h"] = "y";
            
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
        FrameworkElement GetRootTemplateElement(FrameworkElement element) {
            if(element == null)
                return null;

            if(element.TemplatedParent is FrameworkElement) {
                return GetRootTemplateElement(element.TemplatedParent as FrameworkElement);
            }
            return element;
        }
        bool IsHiddenItem(FrameworkElement source, FrameworkElement originalSource) {
            FrameworkElement targetElement = GetRootTemplateElement(originalSource);
            return !Object.Equals(source, targetElement);
        }
        AutomationPeer GetAutomationPeer(UIElement source) {
            return UIElementAutomationPeer.CreatePeerForElement(source);
        }
        void CollectValue(Dictionary<string, string> properties, AutomationPeer automation) {
            if(automation.IsPassword()) {
                properties["Value"] = "*";
            } else if(automation.GetPattern(PatternInterface.Value) is IValueProvider valueProvider) {
                properties["Value"] = valueProvider.Value;
            }
        }
        void LogMouse(IDictionary<string, string> properties, MouseButtonEventArgs e, bool isUp) {
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
            item.CustomData = properties;

            AddBreadcrumb(item);
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
            properties["key"] = isPassword ? Key.Multiply.ToString() : e.Key.ToString();
            properties["SystemKey"] = isPassword ? Key.Multiply.ToString() : e.SystemKey.ToString();
            properties["IsToggled"] = e.IsToggled.ToString();
            properties["IsRepeat"] = e.IsRepeat.ToString();
            properties["KeyStates"] = e.KeyStates.ToString();
            properties["action"] = isUp ? "up" : "down";
            properties["scanCode"] = KeyInterop.VirtualKeyFromKey(isPassword ? Key.Multiply : e.Key).ToString();

            Breadcrumb item = new Breadcrumb();
            item.Event = isUp ? BreadcrumbEvent.KeyUp : BreadcrumbEvent.KeyDown;
            item.CustomData = properties;

            AddBreadcrumb(item);
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
            private set => currentWR = new WeakReference(value);
        }
        public static IInputElement PreviousFocus {
            get { return (IInputElement)oldWR.With(x => x.Target); }
            private set => oldWR = new WeakReference(value);
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
}
