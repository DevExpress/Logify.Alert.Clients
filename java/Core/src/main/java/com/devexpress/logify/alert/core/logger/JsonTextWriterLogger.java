package com.devexpress.logify.alert.core.logger;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class JsonTextWriterLogger implements ILogger {

    private final Map<String, Object> data = new HashMap<String, Object>();
    private final StringBuilder builder;
    private boolean isFirstElement = true;

    public JsonTextWriterLogger(StringBuilder builder) {
        this.builder = builder;
    }

    public StringBuilder getBuilder() {
        return builder;
    }

    public Map<String, Object> getData() {
        return data;
    }

    private static String escapeText(String text) {
        int index;
        for (index = 0; index < text.length(); index++) {
            char c = text.charAt(index);
            if (c < 0x20 || c == '\"' || c == '\\')
                break;
        }

        if (index == text.length())
            return text;

        StringBuilder result = new StringBuilder();
        result.append(text, 0, index);
        for (int i = index; i < text.length(); i++) {
            char c = text.charAt(i);
            if (c == '\"') {
                result.append("\\\"");
                continue;
            }
            if (c == '\\') {
                result.append("\\\\");
                continue;
            }
            if (c >= 0x20) {
                result.append(c);
                continue;
            }
            result.append(String.format("\\u%04x", (int) c));
        }
        return result.toString();
    }

    private void writeName(String name) {
        builder.append("\"")
                .append(replaceDot(name))
                .append("\":");
    }

    public void beginWriteObject(String name) {
        if (!isFirstElement)
            builder.append(",");
        isFirstElement = true;

        if (!name.isEmpty())
            writeName(name);
        builder.append("{");
    }

    public void writeValue(String name, String text) {
        if (!isFirstElement)
            builder.append(",");
        isFirstElement = false;

        writeName(name);
        if(text != null)
            builder.append("\"")
                .append(escapeText(text))
                .append("\"");
        else
            builder.append("null");
    }

    public void writeValue(String name, boolean value) {
        if (!isFirstElement)
            builder.append(",");
        isFirstElement = false;

        writeName(name);
        builder.append(value ? "true" : "false");
    }

    public void writeValue(String name, int value) {
        if (!isFirstElement)
            builder.append(",");
        isFirstElement = false;

        writeName(name);
        builder.append(value);
    }

    public void writeValue(String name, List array) {
        beginWriteArray(name);
        for (int i = 0; i < array.size(); i++) {
            builder.append("\"").append(array.get(i).toString()).append("\"");
            if (i != array.size() - 1)
                builder.append(",");
        }
        endWriteArray(name);
    }

    public void endWriteObject(String name) {
        builder.append("}");

        isFirstElement = false;
    }

    public void beginWriteArray(String name) {
        if (!isFirstElement)
            builder.append(",");
        isFirstElement = true;

        writeName(name);
        builder.append("[");
    }

    public void endWriteArray(String name) {
        builder.append("]");

        isFirstElement = false;
    }

    private String replaceDot(String name) {
        //MongoDB doesn't support keys with a dot in them
        return name.replace('.', '\uff0E');
    }

    @Override
    public String toString() {
        return builder.toString();
    }
}