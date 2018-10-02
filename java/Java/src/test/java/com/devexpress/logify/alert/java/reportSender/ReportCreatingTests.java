package com.devexpress.logify.alert.java.reportSender;

import com.devexpress.logify.alert.core.LogifyClientExceptionReport;
import com.devexpress.logify.alert.java.TestLogifyAlert;
import org.junit.After;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

public class ReportCreatingTests {
    private TestLogifyAlert client;

    @Before
    public void setUp() {
        client = new TestLogifyAlert();
    }

    @After
    public void tearDown() {
        client = null;
    }

    @Test
    public void testReportCreating() {
        client.send(new NullPointerException());
        LogifyClientExceptionReport report = client.getExceptionReportFromSender();
        Assert.assertNotEquals(null, report);
        Assert.assertNotEquals(null, report.getReportString());
        Assert.assertNotEquals("", report.getReportString());
    }
}