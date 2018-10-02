package com.devexpress.logify.alert.java.collectors;

import com.devexpress.logify.alert.core.collectors.CollectorTestsBase;
import com.google.gson.JsonObject;
import org.junit.Test;
import java.awt.Dimension;
import java.awt.Toolkit;
import java.io.IOException;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

public class JavaDisplayCollectorTests extends CollectorTestsBase {

    @Test
    public void testDisplayCollector() throws IOException {
        new JavaDisplayCollector().process(null, logger);

        Toolkit toolkit = Toolkit.getDefaultToolkit();

        JsonObject display = parseJsonReport(true).getAsJsonObject("display");
        assertNotNull(display);

        Dimension screenSize = toolkit.getScreenSize();
        assertEquals((int) screenSize.getWidth(), display.get("width").getAsInt());
        assertEquals((int) screenSize.getHeight(), display.get("height").getAsInt());

        assertEquals(toolkit.getColorModel().getPixelSize(), display.get("colorBits").getAsInt());

        int dpi = toolkit.getScreenResolution();
        assertEquals(dpi, display.get("dpiX").getAsInt());
        assertEquals(dpi, display.get("dpiY").getAsInt());
    }
}