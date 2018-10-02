package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.Attachment;
import com.devexpress.logify.alert.core.utils.Base64Encoder;
import com.devexpress.logify.alert.core.logger.ILogger;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.List;
import java.util.zip.GZIPOutputStream;

public class AttachmentCollector implements IInfoCollector {

    private static final int MAX_TOTAL_SIZE = 3 * 1024 * 1024; //3 MB

    private List<Attachment> attachments;

    public AttachmentCollector(List<Attachment> attachments) {
        this.attachments = attachments;
    }

    @Override
    public void process(Throwable ex, ILogger logger) {
        if (attachments == null || attachments.size() == 0) return;

        int totalSize = 0;

        logger.beginWriteArray("attachments");
        try {
            for (Attachment attachment : attachments) {
                String data = compressAndEncode(attachment);
                if (totalSize + data.length() > MAX_TOTAL_SIZE) continue;
                totalSize += data.length();

                logger.beginWriteObject("");
                logger.writeValue("name", attachment.getName());
                logger.writeValue("mimeType", attachment.getMimeType());
                logger.writeValue("content", data);
                logger.writeValue("compress", "gzip");
                logger.endWriteObject("");
            }
        } finally {
            logger.endWriteArray("attachments");
        }
    }

    private String compressAndEncode(Attachment attachment) {
        try {
            ByteArrayOutputStream bytes = new ByteArrayOutputStream();

            GZIPOutputStream gzip = new GZIPOutputStream(bytes);
            gzip.write(attachment.getContent());
            gzip.close();

            return Base64Encoder.encode(bytes.toByteArray());
        } catch (IOException e) {
            return "";
        }
    }
}