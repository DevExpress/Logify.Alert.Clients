package com.devexpress.logify.alert.core.events;

import com.devexpress.logify.alert.core.LogifyClientBase;

public class AfterReportExceptionLogifyEvent extends LogifyEvent {
    public AfterReportExceptionLogifyEvent(LogifyClientBase source, Throwable exception) {
        super(source, exception);
    }
}