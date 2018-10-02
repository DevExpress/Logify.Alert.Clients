package com.devexpress.logify.alert.core.logger;

import com.devexpress.logify.alert.core.collectors.CollectorTestsBase;
import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonParser;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertTrue;

public class JsonTextWriterLoggerTests extends CollectorTestsBase {

    @Before
    public void setUp() {
        super.setUp();
    }

    @After
    public void tearDown() throws IOException {
        super.tearDown();
    }

    @Test
    public void testWriteObject() {
        logger.beginWriteObject("testObject");
        logger.endWriteObject("testObject");

        JsonElement element = parseJsonReport(true).get("testObject");
        assertNotNull(element);
        assertTrue(element.isJsonObject());
        assertTrue(element.getAsJsonObject().entrySet().isEmpty());
    }

    @Test
    public void testWriteObjectNoName() {
        logger.beginWriteObject("");
        logger.endWriteObject("");

        JsonElement element = parseJsonReport(false);
        assertTrue(element.isJsonObject());
        assertTrue(element.getAsJsonObject().entrySet().isEmpty());
    }

    @Test
    public void testWriteValueString() {
        logger.writeValue("variable", "value");

        JsonElement element = parseJsonReport(true).get("variable");
        assertNotNull(element);
        assertTrue(element.isJsonPrimitive());
        assertEquals("value", element.getAsString());
    }

    @Test
    public void testWriteValueStringSpecialCharacters() {
        String text = " \"\\ \u0001 \u0010\" \n\n\n abc";
        logger.writeValue("variable", text);

        JsonElement element = parseJsonReport(true).get("variable");
        assertNotNull(element);
        assertEquals(text, element.getAsString());
    }

    private void testWriteBoolean(boolean value) {
        logger.writeValue("variable", value);

        JsonElement element = parseJsonReport(true).get("variable");
        assertNotNull(element);
        assertTrue(element.isJsonPrimitive());
        assertEquals(value, element.getAsBoolean());
    }

    @Test
    public void testWriteValueBooleanTrue() {
        testWriteBoolean(true);
    }

    @Test
    public void testWriteValueBooleanFalse() {
        testWriteBoolean(false);
    }

    @Test
    public void testWriteInt() {
        logger.writeValue("variable", 42);

        JsonElement element = parseJsonReport(true).get("variable");
        assertNotNull(element);
        assertTrue(element.isJsonPrimitive());
        assertEquals(42, element.getAsInt());
    }

    @Test
    public void testWriteList() {
        List<Object> list = new ArrayList<Object>();
        list.add(4);
        list.add("test");
        list.add(true);
        logger.writeValue("variable", list);

        JsonElement element = parseJsonReport(true).get("variable");
        assertNotNull(element);
        assertTrue(element.isJsonArray());

        JsonArray array = element.getAsJsonArray();
        assertEquals(list.size(), array.size());
        for (int i = 0; i < list.size(); i++) {
            assertEquals(list.get(i).toString(), array.get(i).getAsString());
        }
    }

    @Test
    public void testWriteArray() {
        logger.beginWriteArray("testArray");
        logger.endWriteArray("testArray");

        JsonElement element = parseJsonReport(true).get("testArray");
        assertNotNull(element);
        assertTrue(element.isJsonArray());
        assertEquals(0, element.getAsJsonArray().size());
    }

    @Test
    public void testWriteMultipleElements() {
        logger.writeValue("key1", 42);
        logger.writeValue("key2", "text");
        logger.writeValue("key3", false);

        logger.beginWriteObject("object");
        logger.writeValue("key4", true);
        logger.endWriteObject("object");

        logger.beginWriteArray("array");
        logger.beginWriteObject("");
        logger.endWriteObject("");
        logger.beginWriteObject("");
        logger.endWriteObject("");
        logger.endWriteArray("array");

        new JsonParser().parse("{" + logger.toString() + "}");
    }

}
