using System;
using System.Collections.Generic;

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

            logger.BeginWriteArray("attachments");
            foreach (Attachment attach in attachments) {
                AttachmentCollector collector = new AttachmentCollector(attach);
                collector.Process(ex, logger);
            }
            logger.EndWriteArray("attachments");
        }

        public class AttachmentCollector : IInfoCollector {
            Attachment attach;

            public AttachmentCollector(Attachment attach) {
                this.attach = attach;
            }

            public void Process(Exception ex, ILogger logger) {
                try {
                    if (attach == null)
                        return;
                    if (attach.Content == null)
                        return;
                    if (String.IsNullOrEmpty(attach.Name))
                        return;

                    logger.BeginWriteObject(String.Empty);
                    try {
                        logger.WriteValue("name", attach.Name);
                        logger.WriteValue("mimeType", attach.MimeType);
                        logger.WriteValue("content", Convert.ToBase64String(attach.Content));
                    }
                    finally {
                        logger.EndWriteObject(String.Empty);
                    }
                }
                catch {
                }
            }
        }
    }
}