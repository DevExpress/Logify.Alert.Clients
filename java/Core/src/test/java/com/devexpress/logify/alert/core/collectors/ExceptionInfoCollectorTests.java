package com.devexpress.logify.alert.core.collectors;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import org.junit.Before;
import org.junit.Test;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.List;

import static junit.framework.TestCase.assertEquals;
import static junit.framework.TestCase.assertNotNull;
import static junit.framework.TestCase.assertNull;

public class ExceptionInfoCollectorTests extends CollectorTestsBase {

    private ExceptionInfoCollector collector;

    @Override
    @Before
    public void setUp() {
        super.setUp();
        collector = new ExceptionInfoCollector();
    }

    @Test
    public void testWriteExceptionType() {
        collector.writeException(new NullPointerException(), logger);

        JsonObject report = parseJsonReport(true);
        assertEquals(NullPointerException.class.getCanonicalName(), report.get("type").getAsString());
    }

    @Test
    public void testWriteExceptionNoMessage() {
        collector.writeException(new NullPointerException(), logger);

        JsonObject report = parseJsonReport(true);
        assertNull(report.get("message"));
        assertNotNull(report.get("type"));
        assertNotNull(report.get("stackTrace"));
        assertNotNull(report.get("normalizedStackTrace"));
    }

    @Test
    public void testWriteExceptionMessage() {
        collector.writeException(new NullPointerException("TestMessage"), logger);

        JsonObject report = parseJsonReport(true);
        assertEquals("TestMessage", report.get("message").getAsString());
    }

    @Test
    public void testWriteStackTrace() {
        Exception exception = new NullPointerException();
        collector.writeException(exception, logger);

        StringWriter expectedStackTrace = new StringWriter();
        exception.printStackTrace(new PrintWriter(expectedStackTrace));
        List<String> expected = splitByLines(expectedStackTrace.toString());

        List<String> actual = splitByLines(parseJsonReport(true).get("stackTrace").getAsString());

        assertEquals(expected, actual);
    }

    @Test
    public void testWriteNormalizedStackTrace() {
        Exception exception = new NullPointerException();
        collector.writeException(exception, logger);

        List<String> actual = splitByLines(parseJsonReport(true).get("normalizedStackTrace").getAsString());
        StackTraceElement[] expected = exception.getStackTrace();
        assertEquals(expected.length, actual.size());
        for (int i = 0; i < expected.length; i++)
            assertEquals(expected[i].toString(), actual.get(i));
    }

    @Test
    public void testWriteNestedExceptions() {
        Exception exception3 = new IllegalAccessException();
        Exception exception2 = new IllegalArgumentException(exception3);
        Exception exception1 = new IllegalStateException(exception2);

        collector.process(exception1, logger);

        JsonArray exceptions = parseJsonReport(true).get("exception").getAsJsonArray();
        assertEquals(3, exceptions.size());
        assertEquals(exception1.getClass().getCanonicalName(),
                exceptions.get(0).getAsJsonObject().get("type").getAsString());
        assertEquals(exception2.getClass().getCanonicalName(),
                exceptions.get(1).getAsJsonObject().get("type").getAsString());
        assertEquals(exception3.getClass().getCanonicalName(),
                exceptions.get(2).getAsJsonObject().get("type").getAsString());
    }
}
