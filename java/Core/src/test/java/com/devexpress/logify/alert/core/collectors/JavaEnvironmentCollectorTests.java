package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.JsonElementHelper;
import com.google.gson.JsonObject;

import org.junit.Test;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

public class JavaEnvironmentCollectorTests extends CollectorTestsBase {

    @Test
    public void testJavaEnvironmentCollecting() {
        new JavaEnvironmentCollector().process(null, logger);

        JsonObject environment = parseJsonReport(true).getAsJsonObject("java_env");
        assertNotNull(environment);
        assertEquals(System.getProperty("java.vm.version"), JsonElementHelper.getValueAsStringOrNull(environment.get("vm_version")));
        assertEquals(System.getProperty("java.vm.vendor"), JsonElementHelper.getValueAsStringOrNull(environment.get("vm_vendor")));
        assertEquals(System.getProperty("file.encoding"), JsonElementHelper.getValueAsStringOrNull(environment.get("encoding")));
        assertEquals(System.getProperty("java.runtime.version"), JsonElementHelper.getValueAsStringOrNull(environment.get("runtime_version")));
        assertEquals(System.getProperty("java.awt.graphicsenv"), JsonElementHelper.getValueAsStringOrNull(environment.get("graphics_env")));
    }
}