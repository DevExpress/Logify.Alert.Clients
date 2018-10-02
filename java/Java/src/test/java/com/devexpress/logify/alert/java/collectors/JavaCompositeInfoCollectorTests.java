package com.devexpress.logify.alert.java.collectors;

import com.devexpress.logify.alert.core.Attachment;
import com.devexpress.logify.alert.core.collectors.CollectorTestsBase;
import com.devexpress.logify.alert.java.TestLogifyAlert;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import org.junit.Before;
import org.junit.Test;
import static junit.framework.TestCase.assertNull;
import static org.junit.Assert.assertNotNull;

public class JavaCompositeInfoCollectorTests extends CollectorTestsBase {
    private TestLogifyAlert client;

    @Override
    @Before
    public void setUp() {
          super.setUp();
        client = new TestLogifyAlert();
    }

    private JsonObject getReport() {
        return new JsonParser()
                .parse(client.getExceptionReportFromSender().getReportString())
                .getAsJsonObject();
    }

    @Test
    public void testAllFieldsArePresent() throws Exception {
        client.setAppName("AppName");
        client.getCustomData().put("key", "value");
        client.getAttachments().add(new Attachment("attachment", "Attachment".getBytes("UTF-8")));
        client.getTags().put("tag", "value");
        client.send(new NullPointerException());

        JsonObject report = getReport();
        assertNotNull(report.get("logifyProtocolVersion"));
        assertNotNull(report.get("logifyReportDateTimeUtc"));
        assertNotNull(report.get("devPlatform"));
        assertNotNull(report.get("logifyApp"));
        assertNotNull(report.get("domainName"));
        assertNotNull(report.get("os"));
        assertNotNull(report.get("memory of JVM"));
        assertNotNull(report.get("currentCulture"));
        assertNotNull(report.get("thread"));
        assertNotNull(report.get("java_env"));
        assertNotNull(report.get("display"));
        assertNotNull(report.get("customData"));
        assertNotNull(report.get("attachments"));
        assertNotNull(report.get("tags"));
        assertNotNull(report.get("exception"));
    }

    @Test
    public void testOptionalFieldsNotPresent() {
        client.send(new NullPointerException());

        JsonObject report = getReport();
        assertNull(report.get("customData"));
        assertNull(report.get("attachments"));
        assertNull(report.get("tags"));
    }
}
