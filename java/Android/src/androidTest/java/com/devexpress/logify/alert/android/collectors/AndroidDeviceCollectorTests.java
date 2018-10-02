package com.devexpress.logify.alert.android.collectors;

import android.os.Build;
import android.support.test.InstrumentationRegistry;
import android.support.test.runner.AndroidJUnit4;

import com.devexpress.logify.alert.core.collectors.CollectorTestsBase;
import com.devexpress.logify.alert.core.JsonElementHelper;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;

import org.junit.Test;
import org.junit.runner.RunWith;

import java.util.regex.Pattern;

import static org.junit.Assert.*;

@RunWith(AndroidJUnit4.class)
public class AndroidDeviceCollectorTests extends CollectorTestsBase {

    @Test
    public void testDeviceCollector() {
        new AndroidDeviceCollector(InstrumentationRegistry.getTargetContext()).process(null, logger);

        JsonObject device = parseJsonReport(true).getAsJsonObject("device");
        assertNotNull(device);
        assertEquals(Build.MODEL, JsonElementHelper.getValueAsStringOrNull(device.get("model")));
        assertEquals(Build.BRAND, JsonElementHelper.getValueAsStringOrNull(device.get("brand")));
        assertEquals(Build.MANUFACTURER, JsonElementHelper.getValueAsStringOrNull(device.get("manufacturer")));
        String supportedABIs = JsonElementHelper.getValueAsStringOrNull(device.get("supportedABIs"));
        if(android.os.Build.VERSION.SDK_INT < 21)
            assertNull(supportedABIs);
        else {
            assertTrue(supportedABIs != null && supportedABIs.length() > 0);
        }
        String totalMemory = JsonElementHelper.getValueAsStringOrNull(device.get("totalMemory"));
        assertTrue(totalMemory != null && Pattern.matches("[0-9]+ Mb", totalMemory));
        String availableMemory = JsonElementHelper.getValueAsStringOrNull(device.get("availableMemory"));
        assertTrue(availableMemory != null && Pattern.matches("[0-9]+ Mb", availableMemory));
        JsonArray features = device.get("features").getAsJsonArray();
        assertTrue(features.size() > 0);
    }
}