package com.devexpress.logify.alert.core.reportSender;

import com.devexpress.logify.alert.core.LogifyClientExceptionReport;

public interface IExceptionReportSender {
    String getServiceUrl();
    void setServiceUrl(String serviceUrl);
    String getApiKey();
    void setApiKey(String apiKey);

    boolean getConfirmSendReport();
    void setConfirmSendReport(boolean confirmSendReport);;

    boolean canSendExceptionReport();
    boolean sendExceptionReport(LogifyClientExceptionReport report);

    IExceptionReportSender clone();
    void copyFrom(IExceptionReportSender instance);
}