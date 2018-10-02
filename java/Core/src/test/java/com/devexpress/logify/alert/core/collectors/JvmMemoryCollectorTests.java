package com.devexpress.logify.alert.core.collectors;

import com.google.gson.JsonObject;
import org.junit.Test;

import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertTrue;

public class JvmMemoryCollectorTests extends CollectorTestsBase {

    @Test
    public void testJvmMemoryCollecting() {
        new JvmMemoryCollector().process(null, logger);

        String memoryPattern = "^\\d+? Mb$";
        JsonObject memory = parseJsonReport(true).getAsJsonObject("memory of JVM");
        assertNotNull(memory);
        assertTrue(memory.get("totalMemory").getAsString().matches(memoryPattern));
        assertTrue(memory.get("freeMemory").getAsString().matches(memoryPattern));
        assertTrue(memory.get("usedMemory").getAsString().matches(memoryPattern));
        assertTrue(memory.get("totalVirtualMemory").getAsString().matches(memoryPattern));
    }
}