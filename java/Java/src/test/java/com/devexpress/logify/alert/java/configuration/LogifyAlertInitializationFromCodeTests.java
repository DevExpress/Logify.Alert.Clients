package com.devexpress.logify.alert.java.configuration;

import com.devexpress.logify.alert.java.LogifyAlert;
import org.junit.Before;
import org.junit.Test;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotEquals;

public class LogifyAlertInitializationFromCodeTests {
    private LogifyAlert client;
    private String testApiKey;
    private String testAppName;
    private String testAppVersion;
    private String testUserId;
    String testServiceUrl;

    @Before
    public void setUp() {
        testApiKey = "1F270824AC4549EBA600DA115C3E7D7C";
        testAppName = "testAppName";
        testAppVersion = "testAppVersion";
        testUserId = "testUserId";
        testServiceUrl = "serviceUrl";
        client = new NonSingletonLogifyAlert();
        client.setApiKey(testApiKey);
        client.setAppName(testAppName);
        client.setAppVersion(testAppVersion);
        client.setUserId(testUserId);
        client.setServiceUrl(testServiceUrl);
    }

    @Test
    public void testLogifyClientInitializationFromCode() {
        assertNotEquals(null, client);
        assertEquals(testApiKey, client.getApiKey());
        assertEquals(testAppName, client.getAppName());
        assertEquals(testAppVersion, client.getAppVersion());
        assertEquals(testUserId, client.getUserId());
        assertEquals(testServiceUrl, client.getServiceUrl());
    }
}