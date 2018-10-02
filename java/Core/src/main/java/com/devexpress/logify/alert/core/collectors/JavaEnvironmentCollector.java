package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;

public class JavaEnvironmentCollector implements IInfoCollector {
    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteObject("java_env");
        try {
            logger.writeValue("vm_version", System.getProperty("java.vm.version"));
            logger.writeValue("vm_vendor", System.getProperty("java.vm.vendor"));
            logger.writeValue("encoding", System.getProperty("file.encoding"));
            logger.writeValue("runtime_version", System.getProperty("java.runtime.version"));
            logger.writeValue("graphics_env", System.getProperty("java.awt.graphicsenv"));
        } finally {
            logger.endWriteObject("java_env");
        }
    }
}