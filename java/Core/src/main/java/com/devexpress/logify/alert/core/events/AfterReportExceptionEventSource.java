package com.devexpress.logify.alert.core.events;

public class AfterReportExceptionEventSource extends EventSourceBase {
    private static AfterReportExceptionEventSource source;

    public static AfterReportExceptionEventSource getSource() {
        if (source != null)
            return source;

        source = new AfterReportExceptionEventSource();
        return source;
    }
}