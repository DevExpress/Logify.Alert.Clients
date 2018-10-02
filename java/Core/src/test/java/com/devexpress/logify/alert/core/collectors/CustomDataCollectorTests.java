package com.devexpress.logify.alert.core.collectors;

import com.google.gson.JsonObject;
import org.junit.Test;

import java.util.HashMap;
import java.util.Map;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

public class CustomDataCollectorTests extends CollectorTestsBase {

    @Test
    public void testCustomDataCollecting() {
        Map<String, String> customData = new HashMap<String, String>();
        customData.put("testKey1", "testValue1");
        customData.put("testKey2", "testValue2");
        CustomDataCollector collector = new CustomDataCollector(customData);
        collector.process(null, logger);

        JsonObject data = parseJsonReport(true).getAsJsonObject("customData");
        assertNotNull(data);
        assertEquals(customData.get("testKey1"), data.get("testKey1").getAsString());
        assertEquals(customData.get("testKey2"), data.get("testKey2").getAsString());
    }
}