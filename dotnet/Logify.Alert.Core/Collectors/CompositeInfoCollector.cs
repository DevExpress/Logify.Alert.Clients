using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace DevExpress.Logify.Core.Internal {
    public abstract class CompositeInfoCollector : IInfoCollector {
        readonly List<IInfoCollector> collectors = new List<IInfoCollector>();

        public IList<IInfoCollector> Collectors { get { return collectors; } }

        protected CompositeInfoCollector(LogifyCollectorContext context) {
            RegisterCollectors(context);
        }

        [HandleProcessCorruptedStateExceptions]
        public virtual void Process(Exception ex, ILogger logger) {
            int count = Collectors.Count;
            for (int i = 0; i < count; i++) {
                try {
                    Collectors[i].Process(ex, logger);
                }
                catch {
                }
            }
        }

        protected abstract void RegisterCollectors(LogifyCollectorContext context);
    }

    public abstract class RootInfoCollector : CompositeInfoCollector {
        protected RootInfoCollector(LogifyCollectorContext context) : base(context) {
        }
    }
}
