using System;
using System.Collections.Generic;
using System.Text;

namespace DevExpress.Office.Utils {
    #region ReplacementItem
    public class ReplacementItem {
        readonly int charIndex;
        readonly string replaceWith;

        public ReplacementItem(int charIndex, string replaceWith) {
            this.charIndex = charIndex;
            this.replaceWith = replaceWith;
        }

        public int CharIndex { get { return charIndex; } }
        public string ReplaceWith { get { return replaceWith; } }
    }
    #endregion
    #region ReplacementInfo
    public class ReplacementInfo {
        readonly List<ReplacementItem> items = new List<ReplacementItem>();
        int deltaLength;

        public void Add(int charIndex, string replaceWith) {
            items.Add(new ReplacementItem(charIndex, replaceWith));
            deltaLength += replaceWith.Length - 1; // 1 == char.Length
        }

        public int DeltaLength { get { return deltaLength; } }
        public IList<ReplacementItem> Items { get { return items; } }
    }
    #endregion

    #region FastCharacterMultiReplacement
    public class FastCharacterMultiReplacement {
        readonly StringBuilder buffer;

        public FastCharacterMultiReplacement(StringBuilder stringBuilder) {
            this.buffer = stringBuilder;
        }

        public ReplacementInfo CreateReplacementInfo(string text, Dictionary<char, string> replaceTable) {
            ReplacementInfo result = null;
            for (int i = text.Length - 1; i >= 0; i--) {
                string replaceWith;
                if (replaceTable.TryGetValue(text[i], out replaceWith)) {
                    if (result == null)
                        result = new ReplacementInfo();
                    result.Add(i, replaceWith);
                }
            }
            return result;
        }
        public string PerformReplacements(string text, ReplacementInfo replacementInfo) {
            if (replacementInfo == null)
                return text;

            //AM: replacements may be implemented more effectively
            buffer.Capacity = Math.Max(buffer.Capacity, text.Length + replacementInfo.DeltaLength);
            buffer.Append(text);
            IList<ReplacementItem> replacementItems = replacementInfo.Items;
            int count = replacementItems.Count;
            for (int i = 0; i < count; i++) {
                ReplacementItem item = replacementItems[i];
                buffer.Remove(item.CharIndex, 1);
                string replaceString = item.ReplaceWith;
                if (!String.IsNullOrEmpty(replaceString))
                    buffer.Insert(item.CharIndex, replaceString);
            }

            string result = buffer.ToString();
            buffer.Length = 0;
            return result;
        }
        public string PerformReplacements(string text, Dictionary<char, string> replaceTable) {
            if (String.IsNullOrEmpty(text))
                return text;
            return PerformReplacements(text, CreateReplacementInfo(text, replaceTable));
        }
    }
    #endregion
}
