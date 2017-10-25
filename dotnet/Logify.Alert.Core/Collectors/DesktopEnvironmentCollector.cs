namespace DevExpress.Logify.Core.Internal {
    public class DesktopEnvironmentCollector : CompositeInfoCollector {
        public DesktopEnvironmentCollector(ILogifyClientConfiguration config)
            : base(config) {
        }

        protected override void RegisterCollectors(ILogifyClientConfiguration config) {
            Collectors.Add(new OperatingSystemCollector());
            Collectors.Add(new VirtualMachineCollector());
            Collectors.Add(new DebuggerCollector());
            Collectors.Add(new MemoryCollector(config));
            Collectors.Add(new DisplayCollector());
            Collectors.Add(new Win32GuiResourcesCollector());
            Collectors.Add(new CurrentCultureCollector());
            Collectors.Add(new CurrentUICultureCollector());
            Collectors.Add(new FrameworkVersionsCollector());
            if (config.CollectScreenshot)
                Collectors.Add(new ScreenshotCollector());
            //etc
        }
    }
}
