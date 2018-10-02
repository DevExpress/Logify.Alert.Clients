package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;

public class JvmMemoryCollector implements IInfoCollector {
    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteObject("memory of JVM");

        try {
            /* Total memory currently in use by the JVM */
            logger.writeValue("totalMemory",
                    String.format("%s Mb", Runtime.getRuntime().totalMemory() / (1024 * 1024)));

            /* Total amount of free memory available to the JVM */
            logger.writeValue("freeMemory",
                    String.format("%s Mb", Runtime.getRuntime().freeMemory() / (1024 * 1024)));

            logger.writeValue("usedMemory",
                    String.format("%s Mb",
                            (Runtime.getRuntime().totalMemory() - Runtime.getRuntime().freeMemory()) / (1024 * 1024)));

            /* This will return Long.MAX_VALUE if there is no preset limit */
            long maxMemory = Runtime.getRuntime().maxMemory();

            /* Maximum amount of memory the JVM will attempt to use */
            logger.writeValue("totalVirtualMemory",
                    String.format("%s Mb",
                            String.valueOf(maxMemory == Long.MAX_VALUE ? "no limit" : maxMemory / (1024 * 1024))));

            //information about heap of JVM memory available
        } finally {
            logger.endWriteObject("memory of JVM");
        }
    }
}