package com.devexpress.logify.alert.java.configuration;

import org.junit.AfterClass;
import org.junit.Before;
import org.junit.Test;

import java.io.IOException;
import java.util.Map;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotEquals;

public class LogifyAlertInitializationFromConfigTests extends InitializationTestBase {
    @AfterClass
    public static void tearDownClass() {
        removeConfig();
    }

    @Before
    public void setUp() throws IOException {
        initConfig();
        initClient();
    }

    @Test
    public void testLogifyClientInitializationFromConfig() {
        assertNotEquals(null, client);
        assertEquals(apiKeyConfig, client.getApiKey());
        assertEquals(appNameConfig, client.getAppName());
        assertEquals(appVersionConfig, client.getAppVersion());
        assertEquals(userIdConfig, client.getUserId());
        assertEquals(serviceUrlConfig, client.getServiceUrl());

        assertEquals(customDataConfig.size(), client.getCustomData().size());
        for (Map.Entry<String, String> entry : customDataConfig.entrySet()) {
            assertEquals(entry.getValue(), client.getCustomData().get(entry.getKey()));
        }
        assertEquals(tagsConfig.size(), client.getTags().size());
        for (Map.Entry<String, String> entry : tagsConfig.entrySet()) {
            assertEquals(entry.getValue(), client.getTags().get(entry.getKey()));
        }
    }
}