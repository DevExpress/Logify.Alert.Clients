package com.devexpress.logify.alert.android.collectors;

import android.support.test.InstrumentationRegistry;
import android.support.test.runner.AndroidJUnit4;

import com.devexpress.logify.alert.core.ApplicationProperties;
import com.devexpress.logify.alert.core.collectors.CollectorTestsBase;
import com.google.gson.JsonObject;

import org.junit.Test;
import org.junit.runner.RunWith;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

@RunWith(AndroidJUnit4.class)
public class AndroidLogifyAppInfoCollectorTests extends CollectorTestsBase {
    private static final String NAME = "testAppName";
    private static final String VERSION = "testAppVersion";
    private static final String USER = "testUserId";
    private static final String DEFAULT_NAME = "com.devexpress.logify.Android.Android.test";

    @Test
    public void testAndroidLogifyAppInfoCollector() {
        ApplicationProperties applicationProperties = new ApplicationProperties();
        applicationProperties.AppName = NAME;
        applicationProperties.AppVersion = VERSION;
        applicationProperties.UserId = USER;
        AndroidLogifyAppInfoCollector collector = new AndroidLogifyAppInfoCollector(InstrumentationRegistry.getTargetContext(), applicationProperties);

        collector.process(null, logger);

        JsonObject logifyApp = parseJsonReport(true).getAsJsonObject("logifyApp");
        assertNotNull(logifyApp);
        assertEquals(NAME, logifyApp.get("name").getAsString());
        assertEquals(VERSION, logifyApp.get("version").getAsString());
        assertEquals(USER, logifyApp.get("userId").getAsString());
    }

    @Test
    public void testAndroidLogifyAppInfoCollectorWithoutCustomInfo() {
        ApplicationProperties applicationProperties = new ApplicationProperties();
        AndroidLogifyAppInfoCollector collector = new AndroidLogifyAppInfoCollector(InstrumentationRegistry.getTargetContext(), applicationProperties);

        collector.process(null, logger);

        JsonObject logifyApp = parseJsonReport(true).getAsJsonObject("logifyApp");
        assertNotNull(logifyApp);
        assertEquals(DEFAULT_NAME, logifyApp.get("name").getAsString());
        assertEquals(null, logifyApp.get("version"));
        assertEquals(null, logifyApp.get("userId"));
    }

    @Test
    public void testAndroidLogifyAppInfoCollectorWithoutCustomInfoAndContext() {
        ApplicationProperties applicationProperties = new ApplicationProperties();
        AndroidLogifyAppInfoCollector collector = new AndroidLogifyAppInfoCollector(null, applicationProperties);

        collector.process(null, logger);

        JsonObject logifyApp = parseJsonReport(true).getAsJsonObject("logifyApp");
        assertNotNull(logifyApp);
        assertEquals(null, logifyApp.get("name"));
        assertEquals(null, logifyApp.get("version"));
        assertEquals(null, logifyApp.get("userId"));
    }
}