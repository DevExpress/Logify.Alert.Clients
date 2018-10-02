package com.devexpress.logify.alert.core.events;

public class BeforeReportExceptionEventSource extends EventSourceBase {
    private static BeforeReportExceptionEventSource source;

    public static BeforeReportExceptionEventSource getSource() {
        if (source != null)
            return source;

        source = new BeforeReportExceptionEventSource();
        return source;
    }
}