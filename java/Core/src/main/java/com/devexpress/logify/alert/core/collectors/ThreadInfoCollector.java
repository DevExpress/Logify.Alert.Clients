package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;

public class ThreadInfoCollector implements IInfoCollector {

    @Override
    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteObject("thread");
        try {
            Thread thread = Thread.currentThread();

            logger.writeValue("id", (int) thread.getId());
            logger.writeValue("name", thread.getName());
            logger.writeValue("priority", thread.getPriority());
            logger.writeValue("state", thread.getState().toString());

            logger.writeValue("threadGroup", thread.getThreadGroup().getName());
        } finally {
            logger.endWriteObject("thread");
        }
    }
}