package com.devexpress.logify.alert.android.collectors;

import android.os.Build;
import android.support.test.runner.AndroidJUnit4;

import com.devexpress.logify.alert.core.collectors.CollectorTestsBase;
import com.google.gson.JsonObject;

import org.junit.Test;
import org.junit.runner.RunWith;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertTrue;

@RunWith(AndroidJUnit4.class)
public class AndroidOperatingSystemCollectorTests extends CollectorTestsBase {

    @Test
    public void testAndroidOperatingSystemCollector(){
        new AndroidOperatingSystemCollector().process(null, logger);

        JsonObject os = parseJsonReport(true).getAsJsonObject("os");
        assertNotNull(os);
        assertEquals("android", os.get("platform").getAsString());
        assertEquals(System.getProperty("os.arch"), os.get("architecture").getAsString());
        assertEquals(System.getProperty("os.version"), os.get("kernelVersion").getAsString());
        assertEquals(android.os.Build.VERSION.RELEASE, os.get("version").getAsString());
        assertEquals(Build.VERSION.SDK_INT, os.get("apiLevel").getAsInt());
        assertEquals(Build.DISPLAY, os.get("buildId").getAsString());
        assertEquals(Build.FINGERPRINT, os.get("buildFingerprint").getAsString());
        assertTrue(os.get("rooted") != null);
    }
}