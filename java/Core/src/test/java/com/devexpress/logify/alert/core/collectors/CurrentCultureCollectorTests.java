package com.devexpress.logify.alert.core.collectors;

import com.google.gson.JsonObject;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import java.io.IOException;
import java.util.Locale;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

public class CurrentCultureCollectorTests extends CollectorTestsBase {

    private CurrentCultureCollector collector;
    private Locale locale;

    @Before
    public void setUp() {
        super.setUp();
        locale = Locale.getDefault();
        Locale.setDefault(Locale.US);
        collector = new CurrentCultureCollector();
    }

    @After
    public void tearDown() throws IOException {
        Locale.setDefault(locale);
        collector = null;
        locale = null;
        super.tearDown();
    }

    @Test
    public void testCultureCollecting() {
        collector.process(null, logger);

        JsonObject culture = parseJsonReport(true).getAsJsonObject("currentCulture");
        assertNotNull(culture);
        assertEquals(locale.getLanguage() + "-" + locale.getCountry(),
                culture.getAsJsonPrimitive("name").getAsString());
        assertEquals(locale.getDisplayName(Locale.US),
                culture.getAsJsonPrimitive("englishName").getAsString());
        assertEquals(locale.getDisplayName(), culture.getAsJsonPrimitive("displayName").getAsString());
    }
}