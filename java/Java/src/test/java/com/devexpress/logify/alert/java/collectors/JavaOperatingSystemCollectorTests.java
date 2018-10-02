package com.devexpress.logify.alert.java.collectors;

import com.devexpress.logify.alert.core.collectors.CollectorTestsBase;
import com.google.gson.JsonObject;

import org.junit.Assert;
import org.junit.Test;

public class JavaOperatingSystemCollectorTests extends CollectorTestsBase {

    @Test
    public void testOperationSystemCollecting() {
        new JavaOperatingSystemCollector().process(null, logger);

        JsonObject os = parseJsonReport(true).getAsJsonObject("os");
        Assert.assertNotNull(os);
        Assert.assertEquals(System.getProperty("os.name"), os.get("platform").getAsString());
        Assert.assertEquals(System.getProperty("os.arch"), os.get("architecture").getAsString());
        Assert.assertEquals(System.getProperty("os.version"), os.get("version").getAsString());
    }
}