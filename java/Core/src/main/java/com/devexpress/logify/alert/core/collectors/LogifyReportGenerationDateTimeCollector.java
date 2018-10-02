package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

public class LogifyReportGenerationDateTimeCollector implements IInfoCollector {
    private static final DateFormat TIME_FORMAT = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");

    static {
        TIME_FORMAT.setTimeZone(TimeZone.getTimeZone("UTC"));
    }

    @Override
    public void process(Throwable ex, ILogger logger) {
        logger.writeValue("logifyReportDateTimeUtc", TIME_FORMAT.format(new Date()));
    }
}