package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.Platform;
import com.google.gson.JsonObject;
import org.junit.Test;

import static org.junit.Assert.assertEquals;

public class PlatformCollectorTests extends CollectorTestsBase {

    private DevelopmentPlatformCollector collector;

    private void checkPlatform(Platform expected) {
        JsonObject platform = parseJsonReport(true);
        assertEquals(expected.toString(), platform.get("platform").getAsString());
    }

    @Test
    public void testJavaPlatformCollecting() {
        collector = new DevelopmentPlatformCollector(Platform.java);
        collector.process(null, logger);
        checkPlatform(Platform.java);
    }

    @Test
    public void testAndroidPlatformCollecting() {
        collector = new DevelopmentPlatformCollector(Platform.android);
        collector.process(null, logger);
        checkPlatform(Platform.android);
    }
}