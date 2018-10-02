package com.devexpress.logify.alert.core;

import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.configuration.ClientConfiguration;
import com.devexpress.logify.alert.core.reportSender.IExceptionReportSender;
import com.devexpress.logify.alert.core.reportSender.ReportSender;
import java.util.Map;

public class TestLogifyClient extends LogifyClientBase {
    private TestReportSender sender;

    public LogifyClientExceptionReport getExceptionReportFromSender() {
        if (sender == null)
            return null;
        return sender.getExceptionReport();
    }

    private class TestReportSender extends ReportSender {

        private LogifyClientExceptionReport report;

        LogifyClientExceptionReport getExceptionReport() {
            return report;
        }

        @Override
        public boolean sendExceptionReport(LogifyClientExceptionReport report) {
            this.report = report;
            return true;
        }

        @Override
        public boolean canSendExceptionReport() {
            return true;
        }

        @Override
        public IExceptionReportSender clone() {
            return this;
        }
    }

    @Override
    protected void configure() {
    }

    @Override
    protected IExceptionReportSender createExceptionReportSender() {
        if (sender == null)
            sender = new TestReportSender();
        return sender;
    }

    @Override
    protected IInfoCollector createDefaultCollector(Map<String, String> additionalCustomData,
                                                    AttachmentCollection additionalAttachments) {
        return null;
    }

    @Override
    protected ClientConfiguration loadConfiguration() {
        return null;
    }
}