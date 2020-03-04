namespace DevExpress.Logify.Core.Internal {
    internal class WinFormsMemoryCollector : MemoryCollector {
        public WinFormsMemoryCollector(LogifyCollectorContext context) : base(context) {}

        protected override void RegisterCollectors(LogifyCollectorContext context) {
            base.RegisterCollectors(context);
            Collectors.Add(new AvailableVirtulaMemoryCollector());
        }
    }
}