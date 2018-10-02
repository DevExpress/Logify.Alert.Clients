package com.devexpress.logify.alert.java;

import com.devexpress.logify.alert.core.LogifyClientExceptionReport;
import com.devexpress.logify.alert.core.reportSender.IExceptionReportSender;
import com.devexpress.logify.alert.core.reportSender.ReportSender;

public class TestLogifyAlert extends LogifyAlert {
    private TestReportSender sender;

    public LogifyClientExceptionReport getExceptionReportFromSender() {
        if (sender == null)
            return null;
        return sender.getExceptionReport();
    }

    private class TestReportSender extends ReportSender {

        LogifyClientExceptionReport report;

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
    protected IExceptionReportSender createExceptionReportSender() {
        if (sender == null)
            sender = new TestReportSender();
        return sender;
    }
}