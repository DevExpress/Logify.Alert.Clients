package com.devexpress.logify.alert.core;

import com.devexpress.logify.alert.core.reportSender.IExceptionReportSender;

public class ExceptionLoggerFactory {
    private static ExceptionLoggerFactory instance;
    private IExceptionReportSender platformReportSender;

    public static ExceptionLoggerFactory getInstance() {
        if (instance != null)
            return instance;

        synchronized (ExceptionLoggerFactory.class) {
            if (instance != null)
                return instance;

            instance = new ExceptionLoggerFactory();
            return instance;
        }
    }

    public IExceptionReportSender getPlatformReportSender() {
        return platformReportSender;
    }
    public void setPlatformReportSender(IExceptionReportSender sender) {
        platformReportSender = sender;
    }
}