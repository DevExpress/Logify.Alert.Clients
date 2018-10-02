package com.devexpress.logify.alert.core.collectors;

import java.util.Locale;

public class CurrentCultureCollector extends CultureBaseCollector {
    public CurrentCultureCollector() {
        super("currentCulture");
    }

    public Locale getLocale() {
        return locale;
    }
}