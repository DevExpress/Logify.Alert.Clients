package com.devexpress.logify.alert.core;

import com.google.gson.JsonElement;

public class JsonElementHelper {
    public static String getValueAsStringOrNull(JsonElement jsonElement) {
        if (jsonElement != null && !jsonElement.isJsonNull())
            return jsonElement.getAsString();
        else
            return null;
    }
}
