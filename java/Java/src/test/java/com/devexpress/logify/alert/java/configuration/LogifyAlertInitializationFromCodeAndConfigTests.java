package com.devexpress.logify.alert.java.configuration;

import org.junit.AfterClass;
import org.junit.Before;
import org.junit.Test;

import java.io.IOException;
import java.util.Map;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotEquals;

public class LogifyAlertInitializationFromCodeAndConfigTests extends InitializationTestBase {
    private String apiKey;
    private String appName;
    private String appVersion;
    private String userId;
    private String serviceUrl;

    @AfterClass
    public static void tearDownClass() {
        removeConfig();
    }

    @Before
    public void setUp() throws IOException {
        initConfig();
        initClient();

        apiKey = "1F270824AC4549EBA600DA115C3E7D7C";
        appName = "Logify.Java";
        appVersion = "0.0.0.1.11";
        userId = "TestUserId";
        serviceUrl = "TestServiceUrl";

        client.setApiKey(apiKey);
        client.setAppName(appName);
        client.setAppVersion(appVersion);
        client.setUserId(userId);
        client.setServiceUrl(serviceUrl);

        client.addCustomData("additionalName", "additionalValue");
        client.addTag("additionalTagName", "additionalTagValue");
    }

    @Test
    public void testLogifyClientInitializationFromCodeAndConfig() {
        assertNotEquals(null, client);
        assertEquals(apiKey, client.getApiKey());
        assertEquals(appName, client.getAppName());
        assertEquals(appVersion, client.getAppVersion());
        assertEquals(userId, client.getUserId());
        assertEquals(serviceUrl, client.getServiceUrl());

        assertEquals(customDataConfig.size() + 1, client.getCustomData().size());
        for (Map.Entry<String, String> entry : customDataConfig.entrySet()) {
            assertEquals(entry.getValue(), client.getCustomData().get(entry.getKey()));
        }
        assertEquals("additionalValue", client.getCustomData().get("additionalName"));

        assertEquals(tagsConfig.size() + 1, client.getTags().size());
        for (Map.Entry<String, String> entry : tagsConfig.entrySet()) {
            assertEquals(entry.getValue(), client.getTags().get(entry.getKey()));
        }
        assertEquals("additionalTagValue", client.getTags().get("additionalTagName"));
    }
}