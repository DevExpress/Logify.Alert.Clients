package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;
import com.devexpress.logify.alert.core.Platform;

public class DevelopmentPlatformCollector implements IInfoCollector {
    private Platform platform = Platform.none;

    public DevelopmentPlatformCollector(Platform platform) {
        this.platform = platform;
    }

    public void process(Throwable ex, ILogger logger) {
        logger.writeValue("devPlatform", "java");
        logger.writeValue("platform", platform.toString());
    }
}