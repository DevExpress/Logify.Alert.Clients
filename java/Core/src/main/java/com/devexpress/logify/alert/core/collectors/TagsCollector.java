package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;
import java.util.Map;

public class TagsCollector implements IInfoCollector {
    private Map<String, String> tags;

    public  TagsCollector(Map<String, String> tags){
        this.tags = tags;
    }

    public void process(Throwable ex, ILogger logger) {
        if (tags == null || tags.size() == 0)
            return;

        logger.beginWriteObject("tags");
        for (String key : tags.keySet())
            logger.writeValue(key, tags.get(key));
        logger.endWriteObject("tags");
    }
}