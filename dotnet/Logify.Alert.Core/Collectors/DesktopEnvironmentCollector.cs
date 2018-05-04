namespace DevExpress.Logify.Core.Internal {
    public class DesktopEnvironmentCollector : CompositeInfoCollector {
        public DesktopEnvironmentCollector(LogifyCollectorContext context)
            : base(context) {
        }

        protected override void RegisterCollectors(LogifyCollectorContext context) {
            Collectors.Add(new OperatingSystemCollector());
            Collectors.Add(new VirtualMachineCollector());
            Collectors.Add(new DebuggerCollector());
            Collectors.Add(new MemoryCollector(context));
            Collectors.Add(new DisplayCollector());
            Collectors.Add(new Win32GuiResourcesCollector());
            Collectors.Add(new CurrentCultureCollector());
            Collectors.Add(new CurrentUICultureCollector());
            Collectors.Add(new FrameworkVersionsCollector());
            Collectors.Add(new EnvironmentCollector());
            if (context.Config != null && context.Config.CollectScreenshot)
                Collectors.Add(new ScreenshotCollector());
            //etc
        }
    }
}
