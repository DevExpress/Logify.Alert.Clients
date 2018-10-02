package com.devexpress.logify.alert.core.configuration;

import java.util.HashMap;
import java.util.Map;
import java.util.Properties;

public abstract class ClientConfigurationLoader {
    private final String PROPERTY_API_KEY = "apiKey";
    private final String PROPERTY_APP_NAME = "appName";
    private final String PROPERTY_APP_VERSION = "appVersion";
    private final String PROPERTY_USER_ID = "userId";
    private final String PROPERTY_SERVICE_URL = "serviceUrl";
    private final String PROPERTY_CUSTOM_DATA = "customData";
    private final String PROPERTY_TAGS = "tags";

    protected final String CONFIGURATION_FILE_NAME = "logify.properties";

    public ClientConfiguration getClientConfiguration() {
        Properties properties = getProperties();
        if (properties != null)
            return getClientConfiguration(properties);
        else
            return null;
    }

    protected abstract Properties getProperties();

    protected ClientConfiguration getClientConfiguration(Properties properties) {
        ClientConfiguration configuration = new ClientConfiguration();

        configuration.ServiceUrl = properties.getProperty(PROPERTY_SERVICE_URL);
        configuration.ApiKey = properties.getProperty(PROPERTY_API_KEY);
        configuration.AppName = properties.getProperty(PROPERTY_APP_NAME);
        configuration.AppVersion = properties.getProperty(PROPERTY_APP_VERSION);
        configuration.UserId = properties.getProperty(PROPERTY_USER_ID);

        String tagsStr = properties.getProperty(PROPERTY_TAGS);
        if (tagsStr != null)
            configuration.Tags = parseMap(tagsStr);

        String customDataStr = properties.getProperty(PROPERTY_CUSTOM_DATA);
        if (customDataStr != null)
            configuration.CustomData = parseMap(customDataStr);

        return  configuration;
    }

    private static Map<String, String> parseMap(String mapStr) {
        Map<String, String> map = new HashMap<String, String>();
        String[] pairs = mapStr.split(",");
        for (String pairStr : pairs) {
            String[] pair = pairStr.split(":");
            if (pair.length == 2)
                map.put(pair[0], pair[1]);
        }
        return map;
    }
}
