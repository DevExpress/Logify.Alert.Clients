package com.devexpress.logify.alert.core.reportSender;

import com.devexpress.logify.alert.core.LogifyClientExceptionReport;
import java.io.IOException;
import java.util.concurrent.ExecutionException;

public abstract class ExceptionReportSenderSkeleton implements IExceptionReportSender {
    private String serviceUrl;
    private String apiKey;
    private int retryCount;
    private int reportTimeoutMilliseconds;
    private boolean confirmSendReport;

    public String getServiceUrl() {
        return serviceUrl;
    }

    public void setServiceUrl(String serviceUrl) {
        this.serviceUrl = serviceUrl;
    }

    public String getApiKey() {
        return apiKey;
    }

    public void setApiKey(String apiKey) {
        this.apiKey = apiKey;
    }

    public boolean getConfirmSendReport() {
        return confirmSendReport;
    }

    public void setConfirmSendReport(boolean confirmSendReport) {
        this.confirmSendReport = confirmSendReport;
    }

    public int getRetryCount() {
        return retryCount;
    }

    public void setRetryCount(int retryCount) {
        this.retryCount = retryCount;
    }

    public int getReportTimeoutMilliseconds() {
        return reportTimeoutMilliseconds;
    }

    public void setReportTimeoutMilliseconds(int reportTimeoutMilliseconds) {
        this.reportTimeoutMilliseconds = reportTimeoutMilliseconds;
    }

    protected ExceptionReportSenderSkeleton() {
        retryCount = 3;
        reportTimeoutMilliseconds = 5000;
    }

    public boolean canSendExceptionReport() {
        return serviceUrl != null && !serviceUrl.isEmpty()
                && apiKey != null && !apiKey.isEmpty();
    }

    public boolean sendExceptionReport(LogifyClientExceptionReport report) {
        for (int i = 0; i < retryCount; i++) {
            try {
                if (sendExceptionReportCore(report))
                    return true;
            } catch (Exception ignored) { }
        }
        return false;
    }

    public abstract boolean sendExceptionReportCore(LogifyClientExceptionReport report)
            throws IOException, ExecutionException, InterruptedException;
    public abstract IExceptionReportSender createEmptyClone();

    public IExceptionReportSender clone() {
        IExceptionReportSender clone = createEmptyClone();
        clone.copyFrom(this);
        return clone;
    }

    public void copyFrom(IExceptionReportSender instance) {
        serviceUrl = instance.getServiceUrl();
        apiKey = instance.getApiKey();
        confirmSendReport = instance.getConfirmSendReport();
    }
}