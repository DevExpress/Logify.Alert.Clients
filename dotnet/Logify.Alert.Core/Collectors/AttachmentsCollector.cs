using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DevExpress.Logify.Core {
    public class AttachmentsCollector : IInfoCollector {
        readonly AttachmentCollection attachments;

        public AttachmentsCollector(AttachmentCollection attachments, AttachmentCollection additionalAttachments) {
            this.attachments = MergeData(attachments, additionalAttachments);
        }

        AttachmentCollection MergeData(AttachmentCollection attachments, AttachmentCollection additionalAttachments) {
            try {
                if (attachments == null || attachments.Count <= 0)
                    return additionalAttachments;
                if (additionalAttachments == null || additionalAttachments.Count <= 0)
                    return attachments;

                AttachmentCollection result = new AttachmentCollection();
                result.AddRange(attachments);
                result.AddRange(additionalAttachments);
                return result;
            }
            catch {
                return attachments;
            }
        }

        public void Process(Exception ex, ILogger logger) {
            if (attachments == null || attachments.Count <= 0)
                return;

            int totalAttachmentSize = 0;
            const int maxTotalAttachmentSize = 3 * 1024 * 1024; // 3Mb
            logger.BeginWriteArray("attachments");
            foreach (Attachment attach in attachments) {
                AttachmentCollector collector = new AttachmentCollector(attach, totalAttachmentSize, maxTotalAttachmentSize, String.Empty);
                int writtenContentSize = collector.PerformProcess(ex, logger);
                totalAttachmentSize += writtenContentSize;
            }
            logger.EndWriteArray("attachments");
        }

        
    }
    public class AttachmentCollector : IInfoCollector {
        readonly Attachment attach;
        int totalAttachmentSize;
        int maxTotalAttachmentSize;
        string nodeName;

        public AttachmentCollector(Attachment attach, int totalAttachmentSize, int maxTotalAttachmentSize, string nodeName) {
            this.attach = attach;
            this.totalAttachmentSize = totalAttachmentSize;
            this.maxTotalAttachmentSize = maxTotalAttachmentSize;
            this.nodeName = nodeName;
        }

        public void Process(Exception ex, ILogger logger) {
            PerformProcess(ex, logger);
        }
        public int PerformProcess(Exception ex, ILogger logger) {
            int writtenContentSize = 0;
            try {
                if (attach == null)
                    return 0;
                if (attach.Content == null)
                    return 0;
                if (String.IsNullOrEmpty(attach.Name))
                    return 0;

                byte[] originalContent = attach.Content;
                string encodedContent = CompressAndEncodeData(originalContent);
                if (String.IsNullOrEmpty(encodedContent))
                    return 0;

                writtenContentSize = encodedContent.Length; // assume base64 content
                if (totalAttachmentSize + writtenContentSize > maxTotalAttachmentSize)
                    return 0; // do not store attach exceeding size limit

                logger.BeginWriteObject(nodeName);
                try {
                    logger.WriteValue("name", attach.Name);
                    logger.WriteValue("mimeType", attach.MimeType);
                    logger.WriteValue("content", encodedContent);
                    logger.WriteValue("compress", "gzip");
                }
                finally {
                    logger.EndWriteObject(nodeName);
                }
            }
            catch {
            }
            return writtenContentSize;
        }
        string CompressAndEncodeData(byte[] data) {
            try {
                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (GZipStream stream = new GZipStream(memoryStream, CompressionMode.Compress, true)) {
                        stream.Write(data, 0, data.Length);
                    }
                    memoryStream.Flush();
#if NETSTANDARD
                        ArraySegment<byte> segment;
                        if (!memoryStream.TryGetBuffer(out segment))
                            return null;
                        if (segment.Offset == 0)
                            return Convert.ToBase64String(segment.Array, 0, (int)memoryStream.Length);
                        else {
                            byte[] buffer = memoryStream.ToArray();
                            return Convert.ToBase64String(buffer, 0, buffer.Length);
                        }
#else
                    byte[] buffer = memoryStream.GetBuffer();
                    return Convert.ToBase64String(buffer, 0, (int)memoryStream.Length);
#endif
                }
            }
            catch {
                return String.Empty;
            }
        }
    }
}