package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;
import java.util.Map;

public class CustomDataCollector implements IInfoCollector {

    private Map<String, String> customData;

    public  CustomDataCollector(Map<String, String> customData){
            this.customData = customData;
    }

    public void process(Throwable ex, ILogger logger) {
        if (customData == null || customData.size() == 0)
            return;

        logger.beginWriteObject("customData");
        for (String key : customData.keySet())
            logger.writeValue(key, customData.get(key));
        logger.endWriteObject("customData");
    }
}