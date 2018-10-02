package com.devexpress.logify.alert.core.collectors;

import com.google.gson.JsonObject;
import org.junit.Test;

import java.util.HashMap;
import java.util.Map;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

public class TagsCollectorTests extends CollectorTestsBase {

    @Test
    public void testTagsCollecting() {
        Map<String, String> tags = new HashMap<String, String>();
        tags.put("testKey1", "testValue1");
        tags.put("testKey2", "testValue2");
        TagsCollector collector = new TagsCollector(tags);
        collector.process(null, logger);

        JsonObject data = parseJsonReport(true).getAsJsonObject("tags");
        assertNotNull(data);
        assertEquals(tags.get("testKey1"), data.get("testKey1").getAsString());
        assertEquals(tags.get("testKey2"), data.get("testKey2").getAsString());
    }
}