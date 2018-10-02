package com.devexpress.logify.alert.core.logger;

import java.util.List;
import java.util.Map;

public interface ILogger {
    StringBuilder getBuilder();
    Map<String, Object> getData();

    void beginWriteObject(String name);
    void writeValue(String name, String text);
    void writeValue(String name, boolean value);
    void writeValue(String name, int value);
    void writeValue(String name, List arrayList);
    void endWriteObject(String name);

    void beginWriteArray(String name);
    void endWriteArray(String name);
}