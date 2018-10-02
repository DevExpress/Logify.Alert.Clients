package com.devexpress.logify.alert.java.collectors;

import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.logger.ILogger;

public class JavaOperatingSystemCollector implements IInfoCollector {
    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteObject("os");
        try {
            logger.writeValue("platform", System.getProperty("os.name"));
            logger.writeValue("version", System.getProperty("os.version"));
            String architecture = System.getProperty("os.arch");
            logger.writeValue("architecture", architecture);
            logger.writeValue("is64bit", architecture.contains("64"));
        } finally {
            logger.endWriteObject("os");
        }
    }
}