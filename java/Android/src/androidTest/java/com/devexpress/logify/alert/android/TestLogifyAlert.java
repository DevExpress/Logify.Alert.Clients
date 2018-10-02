package com.devexpress.logify.alert.android;

import com.devexpress.logify.alert.core.LogifyClientBase;
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

    public static TestLogifyAlert getInstance(){
        LogifyClientBase instance = LogifyClientBase.getInstance();
        if (instance != null && instance instanceof TestLogifyAlert)
            return (TestLogifyAlert)instance;

        synchronized (TestLogifyAlert.class) {
            instance = new TestLogifyAlert();
            setInstance(instance);
            return (TestLogifyAlert)instance;
        }
    }

    public static  void clearInstance(){
        LogifyClientBase.setInstance(null);
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
    protected void configure() {
    }

    @Override
    protected IExceptionReportSender createExceptionReportSender() {
        if (sender == null)
            sender = new TestReportSender();
        return sender;
    }
}