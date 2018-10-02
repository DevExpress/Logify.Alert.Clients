package com.devexpress.logify.alert.java.configuration;

import com.devexpress.logify.alert.java.LogifyAlert;
import com.devexpress.logify.alert.java.TestLogifyAlert;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.Properties;

class InitializationTestBase {
    LogifyAlert client;

    static final String apiKeyConfig = "1F270824AC4549EBA600DA115C3E7D7C.config";
    static final String appNameConfig = "Logify.Java.config";
    static final String appVersionConfig = "0.0.0.1.11.config";
    static final String userIdConfig = "TestUserId.config";
    static final String serviceUrlConfig = "TestServiceUrl.config";
    static Map<String, String> customDataConfig = new HashMap<String, String>();
    static Map<String, String> tagsConfig = new HashMap<String, String>();

    void initClient() {
        client = new TestLogifyAlert();
    }

    static void initConfig() throws IOException {
        File logifyConfigFile = new File("logify.properties");
        FileOutputStream fileOutputStream = new FileOutputStream(logifyConfigFile.getAbsolutePath());
        Properties properties = new Properties();
        properties.put("apiKey", apiKeyConfig);
        properties.put("appName", appNameConfig);
        properties.put("appVersion", appVersionConfig);
        properties.put("userId", userIdConfig);
        properties.put("serviceUrl", serviceUrlConfig);

        customDataConfig.put("name1", "value1");
        customDataConfig.put("name2", "value2");
        customDataConfig.put("name3", "value3");
        properties.put("customData", mapToString(customDataConfig));

        tagsConfig.put("tagName1", "tagValue1");
        tagsConfig.put("tagName2", "tagValue2");
        properties.put("tags", mapToString(tagsConfig));
        properties.store(fileOutputStream, "TestConfig");
        fileOutputStream.flush();
        fileOutputStream.close();
    }

    static String mapToString(Map<String, String> map) {
        String result = "";
        for (Map.Entry<String, String> entry : map.entrySet()) {
            result = result + entry.getKey() + ":" + entry.getValue();
            if (entry.getKey() != new ArrayList<String>(map.keySet()).get(map.size() - 1))
                result = result + ",";
        }
        return result;
    }

    static void removeConfig() {
        File logifyConfigFile = new File("logify.properties");
        if (logifyConfigFile.exists() && logifyConfigFile.isFile()) {
            logifyConfigFile.delete();
        }
    }
}