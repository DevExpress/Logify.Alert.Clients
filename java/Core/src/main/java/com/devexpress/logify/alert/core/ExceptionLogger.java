package com.devexpress.logify.alert.core;

import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.logger.JsonTextWriterLogger;
import com.devexpress.logify.alert.core.reportSender.IExceptionReportSender;

import java.io.IOException;

public class ExceptionLogger {
    private IExceptionReportSender reportSender;

    public static boolean reportException(Throwable ex, IInfoCollector collector) {
        IExceptionReportSender reportSender = ExceptionLoggerFactory.getInstance().getPlatformReportSender();
        if (reportSender == null)
            return false;

        if (collector == null)
            return false;

        reportSender = reportSender.clone();

        ExceptionLogger logger = new ExceptionLogger();
        logger.reportSender = reportSender;
        return logger.performReportException(ex, collector);
    }

    boolean performReportException(Throwable ex, IInfoCollector collector) {
        if (ex == null || collector == null)
            return false;

        try {
            return reportExceptionCore(ex, collector);
        } catch (Exception ignored) {
            return false;
        }
    }

    boolean reportExceptionCore(Throwable ex, IInfoCollector collector) throws IOException {
        if (!shouldSendExceptionReport())
            return false;

        LogifyClientExceptionReport report = createExceptionReport(ex, collector);
        return sendExceptionReport(report);
    }

    LogifyClientExceptionReport createExceptionReport(Throwable ex, IInfoCollector collector) throws IOException {
        StringBuilder content = new StringBuilder();
        JsonTextWriterLogger logger = new JsonTextWriterLogger(content);

        logger.beginWriteObject("");
        try {
            collector.process(ex, logger);
        } finally {
            logger.endWriteObject("");
        }

        LogifyClientExceptionReport report = new LogifyClientExceptionReport();
        report.setReportContent(content);
        report.setData(logger.getData());
        return report;
    }

    boolean sendExceptionReport(LogifyClientExceptionReport report) {
        if (reportSender != null)
            return reportSender.sendExceptionReport(report);
        else
            return true;
    }
    boolean shouldSendExceptionReport() {
        return reportSender != null && reportSender.canSendExceptionReport();
    }
}