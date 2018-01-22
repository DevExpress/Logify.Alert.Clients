using System;
using System.Collections.Generic;

namespace DevExpress.Logify.Core.Internal {
    public class TagsCollector : IInfoCollector {
        readonly IDictionary<string, string> tags;

        public TagsCollector(IDictionary<string, string> tags) {
            this.tags = tags;
        }

        public void Process(Exception ex, ILogger logger) {
            if (tags == null || tags.Count <= 0)
                return;

            logger.BeginWriteObject("tags");
            foreach (string key in tags.Keys)
                logger.WriteValue(key, tags[key]);
            logger.EndWriteObject("tags");
        }
    }
}