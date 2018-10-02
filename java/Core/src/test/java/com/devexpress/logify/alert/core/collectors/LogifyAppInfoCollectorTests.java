package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.ApplicationProperties;
import com.google.gson.JsonObject;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import java.io.IOException;

import static junit.framework.TestCase.assertNull;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

public class LogifyAppInfoCollectorTests extends CollectorTestsBase {

    private static final String NAME = "testAppName";
    private static final String VERSION = "testAppVersion";
    private static final String USER = "testUserId";

    private ApplicationProperties applicationProperties;

    private LogifyAppInfoCollector collector;

    @Before
    public void setUp() {
        super.setUp();
        applicationProperties = new ApplicationProperties();
    }

    @After
    public void tearDown() throws IOException {
        collector = null;
        super.tearDown();
    }

    private JsonObject getData() {
        return parseJsonReport(true).getAsJsonObject("logifyApp");
    }

    private void checkData(JsonObject app, String name, String version, String userId) {
        if (name != null) assertEquals(name, app.get("name").getAsString());
        else assertNull(app.get("name"));

        if (version != null) assertEquals(version, app.get("version").getAsString());
        else assertNull(app.get("version"));

        if (userId != null) assertEquals(userId, app.get("userId").getAsString());
        else assertNull(app.get("userId"));
    }

    @Test
    public void testLogifyAppInfoCollecting() {
        applicationProperties.AppName = NAME;
        applicationProperties.AppVersion = VERSION;
        applicationProperties.UserId = USER;
        collector = new LogifyAppInfoCollector(applicationProperties);

        collector.process(null, logger);

        JsonObject app = getData();
        assertNotNull(app);
        checkData(app, NAME, VERSION, USER);
    }

    @Test
    public void testLogifyAppInfoNameMissing() {
        applicationProperties.AppVersion = VERSION;
        applicationProperties.UserId = USER;
        collector = new LogifyAppInfoCollector(applicationProperties);

        collector.process(null, logger);

        JsonObject app = getData();
        assertNotNull(app);
        checkData(app, null, VERSION, USER);
    }

    @Test
    public void testLogifyAppInfoVersionMissing() {
        applicationProperties.AppName = NAME;
        applicationProperties.UserId = USER;
        collector = new LogifyAppInfoCollector(applicationProperties);

        collector.process(null, logger);

        JsonObject app = getData();
        assertNotNull(app);
        checkData(app, NAME, null, USER);
    }

    @Test
    public void testLogifyAppInfoUserIdMissing() {
        applicationProperties.AppName = NAME;
        applicationProperties.AppVersion = VERSION;
        collector = new LogifyAppInfoCollector(applicationProperties);

        collector.process(null, logger);

        JsonObject app = getData();
        assertNotNull(app);
        checkData(app, NAME, VERSION, null);
    }

    @Test
    public void testLogifyAppInfoMissing() {
        collector = new LogifyAppInfoCollector(applicationProperties);

        collector.process(null, logger);

        JsonObject app = getData();
        assertNotNull(app);
        checkData(app, null, null, null);
    }
}