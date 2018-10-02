package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.Attachment;
import com.devexpress.logify.alert.core.Base64EncoderHelper;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import org.junit.Test;

import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;
import java.util.zip.GZIPInputStream;

import static junit.framework.TestCase.assertEquals;
import static junit.framework.TestCase.assertNotNull;

public class AttachmentCollectorTests extends CollectorTestsBase {

    @Test
    public void testAttachmentCollector() throws IOException {
        List<Attachment> attachments = new ArrayList<Attachment>();
        attachments.add(new Attachment("attachment1", "text/plain", "com/devexpress/logify".getBytes()));
        AttachmentCollector collector = new AttachmentCollector(attachments);
        collector.process(null, logger);

        JsonArray array = parseJsonReport(true).getAsJsonArray("attachments");
        assertNotNull(array);
        assertEquals(1, array.size());

        JsonObject attachment = array.get(0).getAsJsonObject();
        assertEquals("attachment1", attachment.get("name").getAsString());
        assertEquals("text/plain", attachment.get("mimeType").getAsString());
        assertEquals("gzip", attachment.get("compress").getAsString());

        byte[] bytes = Base64EncoderHelper.decodeBase64(attachment.get("content").getAsString());
        GZIPInputStream input = new GZIPInputStream(new ByteArrayInputStream(bytes));
        BufferedReader reader = new BufferedReader(new InputStreamReader(input));
        assertEquals("com/devexpress/logify", reader.readLine());
        reader.close();
    }
}
