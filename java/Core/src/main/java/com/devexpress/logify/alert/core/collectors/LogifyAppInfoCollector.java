package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.ApplicationProperties;
import com.devexpress.logify.alert.core.logger.ILogger;

public class LogifyAppInfoCollector implements IInfoCollector {

    protected ApplicationProperties applicationProperties;

    public LogifyAppInfoCollector(ApplicationProperties applicationProperties){
        this.applicationProperties = applicationProperties;
    }

    protected ApplicationProperties getApplicationProperties(){
        return this.applicationProperties == null ? new ApplicationProperties() : this.applicationProperties;
    }
    protected String getAppName() {
        return getApplicationProperties().AppName;
    }

    protected String getAppVersion() {
        return getApplicationProperties().AppVersion;
    }

    protected String getUserId() {
        return getApplicationProperties().UserId;
    }

    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteObject("logifyApp");
        try {
            String name = getAppName();
            if (name != null)
                logger.writeValue("name", name);
            String version = getAppVersion();
            if (version != null)
                logger.writeValue("version", version);
            String userId = getUserId();
            if (userId != null)
                logger.writeValue("userId", userId);
        } finally {
            logger.endWriteObject("logifyApp");
        }
    }
}