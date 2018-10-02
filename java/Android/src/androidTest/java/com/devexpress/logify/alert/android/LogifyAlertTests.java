package com.devexpress.logify.alert.android;

import android.support.test.InstrumentationRegistry;
import android.support.test.runner.AndroidJUnit4;
import com.devexpress.logify.alert.core.collectors.CollectorTestsBase;
import com.devexpress.logify.alert.core.LogifyClientExceptionReport;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import org.junit.After;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;

@RunWith(AndroidJUnit4.class)
public class LogifyAlertTests extends CollectorTestsBase {
    private static final String NAME = "testAppName";
    private static final String VERSION = "testAppVersion";
    private static final String USER = "testUserId";
    private static final String DEFAULT_NAME = "com.devexpress.logify.Android.Android.test";

    TestLogifyAlert client;

    @Before
    public void setUp() {
        client = TestLogifyAlert.getInstance();
    }

    @After
    public void tearDown() { TestLogifyAlert.clearInstance(); }

    @Test
    public void testLogifyClientSetContext(){
        client.send(new NullPointerException());

        LogifyClientExceptionReport exceptionReport = client.getExceptionReportFromSender();
        String reportJson = exceptionReport.getReportString();
        JsonObject report = new JsonParser().parse(reportJson).getAsJsonObject();

        Assert.assertNull(report.get("logifyApp").getAsJsonObject().get("name"));

        client.setContext(InstrumentationRegistry.getTargetContext());

        client.send(new NullPointerException());

        exceptionReport = client.getExceptionReportFromSender();
        reportJson = exceptionReport.getReportString();
        report = new JsonParser().parse(reportJson).getAsJsonObject();

        Assert.assertEquals(DEFAULT_NAME, report.get("logifyApp").getAsJsonObject().get("name").getAsString());
    }

    @Test
    public void testLogifyClientSetAppName(){
        client.send(new NullPointerException());

        LogifyClientExceptionReport exceptionReport = client.getExceptionReportFromSender();
        String reportJson = exceptionReport.getReportString();
        JsonObject report = new JsonParser().parse(reportJson).getAsJsonObject();

        Assert.assertNull(report.get("logifyApp").getAsJsonObject().get("version"));

        client.setAppName(NAME);

        client.send(new NullPointerException());

        exceptionReport = client.getExceptionReportFromSender();
        reportJson = exceptionReport.getReportString();
        report = new JsonParser().parse(reportJson).getAsJsonObject();

        Assert.assertEquals(NAME, report.get("logifyApp").getAsJsonObject().get("name").getAsString());
    }

    @Test
    public void testLogifyClientSetAppVersion(){
        client.send(new NullPointerException());

        LogifyClientExceptionReport exceptionReport = client.getExceptionReportFromSender();
        String reportJson = exceptionReport.getReportString();
        JsonObject report = new JsonParser().parse(reportJson).getAsJsonObject();

        Assert.assertNull(report.get("logifyApp").getAsJsonObject().get("version"));

        client.setAppVersion(VERSION);

        client.send(new NullPointerException());

        exceptionReport = client.getExceptionReportFromSender();
        reportJson = exceptionReport.getReportString();
        report = new JsonParser().parse(reportJson).getAsJsonObject();

        Assert.assertEquals(VERSION, report.get("logifyApp").getAsJsonObject().get("version").getAsString());
    }

    @Test
    public void testLogifyClientSetUserId(){
        client.send(new NullPointerException());

        LogifyClientExceptionReport exceptionReport = client.getExceptionReportFromSender();
        String reportJson = exceptionReport.getReportString();
        JsonObject report = new JsonParser().parse(reportJson).getAsJsonObject();

        Assert.assertNull(report.get("logifyApp").getAsJsonObject().get("version"));

        client.setUserId(USER);

        client.send(new NullPointerException());

        exceptionReport = client.getExceptionReportFromSender();
        reportJson = exceptionReport.getReportString();
        report = new JsonParser().parse(reportJson).getAsJsonObject();

        Assert.assertEquals(USER, report.get("logifyApp").getAsJsonObject().get("userId").getAsString());
    }
}