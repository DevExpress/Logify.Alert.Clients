package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;
import com.devexpress.logify.alert.core.logger.JsonTextWriterLogger;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import org.junit.After;
import org.junit.Before;
import org.junit.Ignore;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.List;

@Ignore
public class CollectorTestsBase {
    protected ILogger logger;

    @Before
    public void setUp() {
        logger = new JsonTextWriterLogger(new StringBuilder());
    }

    @After
    public void tearDown() throws IOException {
        logger = null;
    }

    protected JsonObject parseJsonReport(boolean wrapByRootObject) {
        String json = logger.toString();
        if (wrapByRootObject) json = "{" + json + "}";
        return new JsonParser().parse(json).getAsJsonObject();
    }

    protected List<String> splitByLines(String text) {
        List<String> lines = new ArrayList<String>();
        try {
            BufferedReader reader = new BufferedReader(new StringReader(text));
            String line;
            while ((line = reader.readLine()) != null) lines.add(line);
            reader.close();
        } catch (IOException e) {
            throw new RuntimeException("Caught an IOException, which should never happen");
        }
        return lines;
    }
}