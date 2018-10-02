package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;
import com.devexpress.logify.alert.core.LogifyClientInfo;

public class LogifyProtocolVersionCollector implements IInfoCollector {
    public void process(Throwable ex, ILogger logger) {
        logger.writeValue("logifyProtocolVersion", LogifyClientInfo.VERSION);
    }
}