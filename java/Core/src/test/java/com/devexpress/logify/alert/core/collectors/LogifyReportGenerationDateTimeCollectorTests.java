package com.devexpress.logify.alert.core.collectors;

import com.google.gson.JsonElement;
import org.joda.time.DateTime;
import org.junit.Test;

import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertTrue;

public class LogifyReportGenerationDateTimeCollectorTests extends CollectorTestsBase {

    @Test
    public void testReportGenerationDateTimeCollecting() {
        DateTime start = DateTime.now();
        new LogifyReportGenerationDateTimeCollector().process(null, logger);
        DateTime end = DateTime.now();

        JsonElement dateTime = parseJsonReport(true).get("logifyReportDateTimeUtc");
        assertNotNull(dateTime);

        DateTime actual = new DateTime(dateTime.getAsString());
        assertTrue(actual.isAfter(start) || actual.isEqual(start));
        assertTrue(actual.isBefore(end) || actual.isEqual(end));
    }
}
