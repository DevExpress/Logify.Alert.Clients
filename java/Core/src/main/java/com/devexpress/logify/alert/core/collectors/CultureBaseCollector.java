package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;
import java.util.Locale;

public class CultureBaseCollector implements IInfoCollector {
    final Locale locale;
    final String name;

    protected CultureBaseCollector(String name) {
        this.locale = Locale.getDefault();
        this.name = name;
    }

    public CultureBaseCollector(Locale locale, String name) {
        this.locale = locale;
        this.name = name;
    }

    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteObject(name);
        try {
            String isoName = locale.getLanguage() + "-" + locale.getCountry();

            logger.writeValue("name", isoName);
            logger.writeValue("englishName", locale.getDisplayName(Locale.US));
            logger.writeValue("displayName", locale.getDisplayName());
        } finally {
            logger.endWriteObject(name);
        }
    }
}