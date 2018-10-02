package com.devexpress.logify.alert.core;

import com.devexpress.logify.alert.core.ApplicationProperties;
import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.configuration.ClientConfiguration;
import com.devexpress.logify.alert.core.events.AfterReportExceptionLogifyEvent;
import com.devexpress.logify.alert.core.events.AfterReportExceptionEventSource;
import com.devexpress.logify.alert.core.events.BeforeReportExceptionLogifyEvent;
import com.devexpress.logify.alert.core.events.BeforeReportExceptionEventSource;
import com.devexpress.logify.alert.core.events.CanReportExceptionLogifyEvent;
import com.devexpress.logify.alert.core.events.CanReportExceptionEventSource;
import com.devexpress.logify.alert.core.events.LogifyEventListener;
import com.devexpress.logify.alert.core.reportSender.IExceptionReportSender;
import com.devexpress.logify.alert.core.reportSender.ReportSender;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

public abstract class LogifyClientBase {
    private final String defaultServiceUrl = "http://logify.devexpress.com";
    private String apiKey;
    private ApplicationProperties appProperties;

    public String getServiceUrl() {
        IExceptionReportSender sender = ExceptionLoggerFactory.getInstance().getPlatformReportSender();
        if (sender != null)
            return sender.getServiceUrl();
        else
            return defaultServiceUrl;
    }

    public void setServiceUrl(String serviceUrl) {
        IExceptionReportSender sender = ExceptionLoggerFactory.getInstance().getPlatformReportSender();
        if (sender != null)
            sender.setServiceUrl(serviceUrl);
    }

    public String getApiKey() {
        return apiKey;
    }

    public void setApiKey(String apiKey) {
        this.apiKey = apiKey;
        IExceptionReportSender sender = ExceptionLoggerFactory.getInstance().getPlatformReportSender();
        if (sender != null)
            sender.setApiKey(apiKey);
    }

    protected ApplicationProperties getAppProperties() {
        if (this.appProperties == null)
            this.appProperties = new ApplicationProperties();
        return this.appProperties;
    }

    public String getAppName() {
        return getAppProperties().AppName;
    }

    public void setAppName(String appName) {
        getAppProperties().AppName = appName;
    }

    public String getAppVersion() {
        return getAppProperties().AppVersion;
    }

    public void setAppVersion(String appVersion) {
        getAppProperties().AppVersion = appVersion;
    }

    public String getUserId() {
        return getAppProperties().UserId;
    }

    public void setUserId(String userId) {
        getAppProperties().UserId = userId;
    }

    private static volatile LogifyClientBase instance = null;

    private Thread.UncaughtExceptionHandler defaultHandler;
    protected Map<String, String> customData = new HashMap<String, String>();
    protected Map<String, String> tags = new HashMap<String, String>();
    protected AttachmentCollection attachments = new AttachmentCollection();

    public static LogifyClientBase getInstance() {
        return LogifyClientBase.instance;
    }

    protected static void setInstance(LogifyClientBase instance) {
        LogifyClientBase.instance = instance;
    }

    protected LogifyClientBase() {
        IExceptionReportSender reportSender = createExceptionReportSender();
        reportSender.setServiceUrl(defaultServiceUrl);
        ExceptionLoggerFactory.getInstance().setPlatformReportSender(reportSender);

        configure();
    }

    protected void configure() {
        ClientConfiguration configuration = loadConfiguration();
        if (configuration != null)
            applyConfiguration(configuration);
    }

    protected abstract ClientConfiguration loadConfiguration();

    protected void applyConfiguration(ClientConfiguration configuration) {
        if (configuration.ServiceUrl != null && getServiceUrl().equals(defaultServiceUrl))
            setServiceUrl(configuration.ServiceUrl);
        if (configuration.ApiKey != null && getApiKey() == null)
            setApiKey(configuration.ApiKey);
        if (configuration.AppName != null && getAppName() == null)
            setAppName(configuration.AppName);
        if (configuration.AppVersion != null && getAppVersion() == null)
            setAppVersion(configuration.AppVersion);
        if (configuration.UserId != null && getUserId() == null)
            setUserId(configuration.UserId);
        if (configuration.CustomData != null)
            getCustomData().putAll(configuration.CustomData);
        if (configuration.Tags != null)
            getTags().putAll(configuration.Tags);
    }

    public Map<String, String> getCustomData() {
        return customData;
    }

    public void addCustomData(String name, String value) {
        customData.put(name, value);
    }

    public Map<String, String> getTags() {
        return tags;
    }

    public void addTag(String name, String value) {
        tags.put(name, value);
    }

    public List<Attachment> getAttachments() {
        return attachments;
    }

    public void addAfterReportExceptionListener(LogifyEventListener<AfterReportExceptionLogifyEvent> LogifyEventListener) {
        AfterReportExceptionEventSource.getSource().addListener(LogifyEventListener);
    }

    public void removeAfterReportExceptionListener(LogifyEventListener<AfterReportExceptionLogifyEvent> LogifyEventListener) {
        AfterReportExceptionEventSource.getSource().addListener(LogifyEventListener);
    }

    public void addBeforeReportExceptionListener(LogifyEventListener<BeforeReportExceptionLogifyEvent> LogifyEventListener) {
        BeforeReportExceptionEventSource.getSource().addListener(LogifyEventListener);
    }

    public void removeBeforeReportExceptionListener(LogifyEventListener<BeforeReportExceptionLogifyEvent> LogifyEventListener) {
        BeforeReportExceptionEventSource.getSource().addListener(LogifyEventListener);
    }

    public void addCanReportExceptionListener(LogifyEventListener<CanReportExceptionLogifyEvent> LogifyEventListener) {
        CanReportExceptionEventSource.getSource().addListener(LogifyEventListener);
    }

    public void removeCanReportExceptionListener(LogifyEventListener<CanReportExceptionLogifyEvent> LogifyEventListener) {
        CanReportExceptionEventSource.getSource().removeListener(LogifyEventListener);
    }

    boolean raiseCanReportException(Throwable ex) {
        if (CanReportExceptionEventSource.getSource().getSize() > 0) {
            CanReportExceptionLogifyEvent event = new CanReportExceptionLogifyEvent(this, ex);
            CanReportExceptionEventSource.getSource().fireEvent(event);
            return !event.getCancel();
        }
        return true;
    }

    void raiseAfterReportException(Throwable ex) {
        if (AfterReportExceptionEventSource.getSource().getSize() > 0) {
            AfterReportExceptionLogifyEvent event = new AfterReportExceptionLogifyEvent(this, ex);
            AfterReportExceptionEventSource.getSource().fireEvent(event);
        }
    }

    void raiseBeforeReportException(Throwable ex) {
        if (BeforeReportExceptionEventSource.getSource().getSize() > 0) {
            BeforeReportExceptionLogifyEvent event = new BeforeReportExceptionLogifyEvent(this, ex);
            BeforeReportExceptionEventSource.getSource().fireEvent(event);
        }
    }

    private Thread.UncaughtExceptionHandler exceptionHandler;

    void run() {
        if (this.exceptionHandler == null) {
            exceptionHandler = new Thread.UncaughtExceptionHandler() {
                @Override
                public void uncaughtException(Thread t, Throwable e) {
                    if (e != null)
                        send(e);
                    if (defaultHandler != null)
                        defaultHandler.uncaughtException(t, e);
                }
            };
            defaultHandler = Thread.getDefaultUncaughtExceptionHandler();
            Thread.setDefaultUncaughtExceptionHandler(exceptionHandler);
        }
    }

    void stop() {
        exceptionHandler = null;
        Thread.setDefaultUncaughtExceptionHandler(defaultHandler);
    }

    public void startExceptionsHandling() {
        run();
    }

    public void stopExceptionsHandling() {
        stop();
    }

    protected IExceptionReportSender createExceptionReportSender() {
        return new ReportSender();
    }

    public void send(Throwable ex) {
        reportException(ex, null, null);
    }

    public void send(Throwable ex, Map<String, String> additionalCustomData) {
        reportException(ex, additionalCustomData, null);
    }

    public void send(Throwable ex, Map<String, String> additionalCustomData,
                     AttachmentCollection additionalAttachments) {
        reportException(ex, additionalCustomData, additionalAttachments);
    }

    protected boolean reportException(Throwable ex, Map<String, String> additionalCustomData,
                                      AttachmentCollection additionalAttachments) {
        try {
            if (!raiseCanReportException(ex))
                return false;

            raiseBeforeReportException(ex);

            boolean success = ExceptionLogger.reportException(ex,
                    createDefaultCollector(additionalCustomData, additionalAttachments));

            raiseAfterReportException(ex);
            return success;
        } catch (Exception ignored) {
            return false;
        }
    }

    protected abstract IInfoCollector createDefaultCollector(Map<String, String> additionalCustomData,
                                                             AttachmentCollection additionalAttachments);
}