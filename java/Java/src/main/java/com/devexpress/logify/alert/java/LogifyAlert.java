package com.devexpress.logify.alert.java;

import com.devexpress.logify.alert.core.Attachment;
import com.devexpress.logify.alert.core.AttachmentCollection;
import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.configuration.ClientConfiguration;
import com.devexpress.logify.alert.java.collectors.JavaCompositeInfoCollector;
import com.devexpress.logify.alert.java.configuration.JavaClientConfigurationLoader;
import com.devexpress.logify.alert.core.LogifyClientBase;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class LogifyAlert extends LogifyClientBase {
    private static volatile LogifyAlert instance = null;

    public static LogifyAlert getInstance() {
        LogifyClientBase instance = LogifyClientBase.getInstance();
        if (instance != null && instance instanceof LogifyAlert)
            return (LogifyAlert)instance;

        synchronized (LogifyAlert.class) {
            instance = new LogifyAlert();
            setInstance(instance);
            return (LogifyAlert)instance;
        }
    }

    protected LogifyAlert() {
        super();
    }

    @Override
    protected ClientConfiguration loadConfiguration() {
        return new JavaClientConfigurationLoader().getClientConfiguration();
    }

    @Override
    protected void applyConfiguration(ClientConfiguration configuration){
        super.applyConfiguration(configuration);
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

        return new JavaCompositeInfoCollector(this.getAppProperties(), customData, attachments, this.tags);
    }
}