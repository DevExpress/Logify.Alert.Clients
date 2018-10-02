package com.devexpress.logify.alert.core.events;

public class CanReportExceptionEventSource extends EventSourceBase {
    private static CanReportExceptionEventSource source;

    public static CanReportExceptionEventSource getSource() {
        if (source != null)
            return source;

        source = new CanReportExceptionEventSource();
        return source;
    }
}