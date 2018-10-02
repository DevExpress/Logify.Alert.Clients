package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;

import java.io.IOException;

public interface IInfoCollector {
    void process(Throwable ex, ILogger logger) throws IOException;
}