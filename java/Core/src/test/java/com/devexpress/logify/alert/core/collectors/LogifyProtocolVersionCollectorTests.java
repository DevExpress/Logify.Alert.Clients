package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.LogifyClientInfo;
import org.junit.Test;

import static org.junit.Assert.assertEquals;

public class LogifyProtocolVersionCollectorTests extends CollectorTestsBase {

    @Test
    public void testLogifyProtocolVersionCollecting() {
        new LogifyProtocolVersionCollector().process(null, logger);
        assertEquals(LogifyClientInfo.VERSION, parseJsonReport(true).get("logifyProtocolVersion").getAsString());
    }
}