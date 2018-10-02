package com.devexpress.logify.alert.logback;

import ch.qos.logback.classic.spi.ILoggingEvent;
import ch.qos.logback.classic.spi.IThrowableProxy;
import ch.qos.logback.classic.spi.ThrowableProxy;
import ch.qos.logback.core.AppenderBase;
import com.devexpress.logify.alert.java.LogifyAlert;

public class LogifyAlertAppender extends AppenderBase<ILoggingEvent> {
    private String ApiKey = "";

    public void setApiKey(String apiKey) {
        ApiKey = apiKey;
    }

    @Override
    protected void append(ILoggingEvent eventObject) {
        IThrowableProxy proxy = eventObject.getThrowableProxy();
        if (proxy instanceof ThrowableProxy) {
            Throwable throwable = ((ThrowableProxy) proxy).getThrowable();
            LogifyAlert client = LogifyAlert.getInstance();

            if (client != null) {
                if (client.getApiKey() == null || client.getApiKey().equals(""))
                    client.setApiKey(ApiKey);

                client.send(throwable);
            }
        }
    }
}