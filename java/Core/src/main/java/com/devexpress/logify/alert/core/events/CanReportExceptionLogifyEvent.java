package com.devexpress.logify.alert.core.events;

import com.devexpress.logify.alert.core.LogifyClientBase;

public class CanReportExceptionLogifyEvent extends LogifyEvent {
    private boolean cancel;

    public CanReportExceptionLogifyEvent(LogifyClientBase source, Throwable exception) {
        super(source, exception);
        cancel = false;
    }

    public boolean getCancel() {
        return cancel;
    }

    public void setCancel(boolean cancel) {
        this.cancel = cancel;
    }
}