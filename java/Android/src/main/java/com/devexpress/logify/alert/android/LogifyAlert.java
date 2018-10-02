package com.devexpress.logify.alert.android;

import android.content.Context;

import com.devexpress.logify.alert.core.Attachment;
import com.devexpress.logify.alert.core.AttachmentCollection;
import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.configuration.ClientConfiguration;
import com.devexpress.logify.alert.core.LogifyClientBase;
import com.devexpress.logify.alert.core.reportSender.IExceptionReportSender;
import com.devexpress.logify.alert.android.collectors.AndroidCompositeInfoCollector;
import com.devexpress.logify.alert.android.configuration.AndroidClientConfigurationLoader;
import com.devexpress.logify.alert.android.reportSender.AndroidReportSender;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class LogifyAlert extends LogifyClientBase {

    public static LogifyAlert getInstance() {
        LogifyClientBase instance = LogifyClientBase.getInstance();
        if (instance != null && instance instanceof LogifyAlert)
            return (LogifyAlert) instance;

        synchronized (LogifyAlert.class) {
            instance = new LogifyAlert();
            setInstance(instance);
            return (LogifyAlert) instance;
        }
    }

    protected LogifyAlert() {
        super();
    }

    private Context context;

    public void setContext(Context context) {
        this.context = context;
        configure();
    }

    public Context getContext() {
        return this.context;
    }

    @Override
    protected ClientConfiguration loadConfiguration() {
        return new AndroidClientConfigurationLoader(getContext()).getClientConfiguration();
    }

    @Override
    protected IInfoCollector createDefaultCollector(Map<String, String> additionalCustomData,
                                                    AttachmentCollection additionalAttachments) {
        Map<String, String> customData = new HashMap<String, String>();
        customData.putAll(this.customData);
        if (additionalCustomData != null)
            customData.putAll(additionalCustomData);

        List<Attachment> attachments = new ArrayList<Attachment>();
        attachments.addAll(this.attachments);
        if (additionalAttachments != null)
            attachments.addAll(additionalAttachments);

        return new AndroidCompositeInfoCollector(this.getContext(), this.getAppProperties(), customData, attachments, this.tags);
    }

    @Override
    protected IExceptionReportSender createExceptionReportSender() {
        return new AndroidReportSender();
    }
}