package com.devexpress.logify.alert.core.events;

import com.devexpress.logify.alert.core.TestLogifyClient;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import static org.junit.Assert.assertEquals;

public class EventRisingTests {
    private TestLogifyClient client;
    private AfterReportExceptionEventListener afterReportExceptionEvent;
    private BeforeReportExceptionEventListener beforeReportExceptionEvent;
    private CanReportExceptionEventListener canReportExceptionEvent;
    private int eventCounter;

    @Before
    public void setUp() {
        client = new TestLogifyClient();
        afterReportExceptionEvent = new AfterReportExceptionEventListener();
        beforeReportExceptionEvent = new BeforeReportExceptionEventListener();
        canReportExceptionEvent = new CanReportExceptionEventListener();
        client.addAfterReportExceptionListener(afterReportExceptionEvent);
        client.addBeforeReportExceptionListener(beforeReportExceptionEvent);
        client.addCanReportExceptionListener(canReportExceptionEvent);
        eventCounter = 0;
    }

    @After
    public void tearDown() {
        client.removeAfterReportExceptionListener(afterReportExceptionEvent);
        client.removeBeforeReportExceptionListener(beforeReportExceptionEvent);
        client.removeCanReportExceptionListener(canReportExceptionEvent);
        afterReportExceptionEvent = null;
        beforeReportExceptionEvent = null;
        canReportExceptionEvent = null;
        client = null;
        eventCounter = 0;
    }

    @Test
    public void testEventRising() {
        client.send(new NullPointerException());
        assertEquals(3, eventCounter);
    }

    class AfterReportExceptionEventListener implements LogifyEventListener<AfterReportExceptionLogifyEvent> {
        public void handle(AfterReportExceptionLogifyEvent event) {
            eventCounter += 1;
        }
    }

    class BeforeReportExceptionEventListener implements LogifyEventListener<BeforeReportExceptionLogifyEvent> {
        public void handle(BeforeReportExceptionLogifyEvent event) {
            eventCounter += 1;
        }
    }

    class CanReportExceptionEventListener implements LogifyEventListener<CanReportExceptionLogifyEvent> {
        public void handle(CanReportExceptionLogifyEvent event) {
            eventCounter += 1;
        }
    }
}