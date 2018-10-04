package com.devexpress.logify.alert.log4j;

import com.devexpress.logify.alert.java.LogifyAlert;
import org.apache.log4j.AppenderSkeleton;
import org.apache.log4j.spi.LoggingEvent;

public class LogifyAlertAppender extends AppenderSkeleton {
    @Override
    public void append(LoggingEvent loggingEvent) {
        if (loggingEvent != null && loggingEvent.getThrowableInformation() != null
                && loggingEvent.getThrowableInformation().getThrowable() != null) {
            LogifyAlert client = LogifyAlert.getInstance();

            if (client != null) {
                client.send(loggingEvent.getThrowableInformation().getThrowable());
            }
        }
    }

    @Override
    public void close() { }

    @Override
    public boolean requiresLayout() {
        return false;
    }
}
