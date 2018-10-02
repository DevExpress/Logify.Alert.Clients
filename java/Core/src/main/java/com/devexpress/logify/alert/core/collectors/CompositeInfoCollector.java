package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;

import java.util.ArrayList;
import java.util.List;

public abstract class CompositeInfoCollector implements IInfoCollector {
    protected List<IInfoCollector> collectors = new ArrayList<IInfoCollector>();

    protected CompositeInfoCollector() {
    }

    public void process(Throwable ex, ILogger logger) {
        for (IInfoCollector collector : collectors) {
            try {
                collector.process(ex, logger);
            } catch (Exception ignored) {
            }
        }
    }
}
