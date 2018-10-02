package com.devexpress.logify.alert.core.events;

import com.devexpress.logify.alert.core.LogifyClientBase;

public class BeforeReportExceptionLogifyEvent extends LogifyEvent {
    public BeforeReportExceptionLogifyEvent(LogifyClientBase source, Throwable exception) {
        super(source, exception);
    }
}