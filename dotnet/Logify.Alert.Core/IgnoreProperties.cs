using System;
using System.Collections.Generic;
using System.Globalization;

namespace DevExpress.Logify.Core.Internal {
    public class IgnorePropertiesInfo {
        public bool IgnoreAll { get; set; }
        public HashSet<string> IgnoreNamesExact { get; set; }
        public List<string> IgnoreNamesStartsWith { get; set; }
        public List<string> IgnoreNamesEndsWith { get; set; }
        public List<string> IgnoreNamesContains { get; set; }

        public bool ShouldIgnore(string name) {
            if (IgnoreAll)
                return true;

            if (String.IsNullOrEmpty(name))
                return false;

            return ShouldIgnoreExact(name) ||
                ShouldIgnoreStartsWith(name) ||
                ShouldIgnoreEndsWith(name) ||
                ShouldIgnoreContains(name);
        }
        bool ShouldIgnoreExact(string name) {
            return IgnoreNamesExact != null && IgnoreNamesExact.Contains(name);
        }
        bool ShouldIgnoreStartsWith(string name) {
            if (IgnoreNamesStartsWith == null)
                return false;

            int count = IgnoreNamesStartsWith.Count;
            for (int i = 0; i < count; i++) {
                //if (name.StartsWith(IgnoreNamesStartsWith[i], StringComparison.InvariantCultureIgnoreCase))
                if (name.StartsWith(IgnoreNamesStartsWith[i], StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
        bool ShouldIgnoreEndsWith(string name) {
            if (IgnoreNamesEndsWith == null)
                return false;

            int count = IgnoreNamesEndsWith.Count;
            for (int i = 0; i < count; i++) {
                if (name.EndsWith(IgnoreNamesEndsWith[i], StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
        bool ShouldIgnoreContains(string name) {
            if (IgnoreNamesContains == null)
                return false;

            int count = IgnoreNamesContains.Count;
            for (int i = 0; i < count; i++) {
                if (name.IndexOf(IgnoreNamesContains[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }
        public static IgnorePropertiesInfo FromString(string value) {
            IgnorePropertiesInfo result = new IgnorePropertiesInfo();
            result.IgnoreNamesExact = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            result.IgnoreNamesStartsWith = new List<string>();
            result.IgnoreNamesEndsWith = new List<string>();
            result.IgnoreNamesContains = new List<string>();
            try {
                if (String.IsNullOrEmpty(value))
                    return result;

                string[] names = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (names == null || names.Length <= 0)
                    return result;

                int count = names.Length;
                for (int i = 0; i < count; i++) {
                    string item = names[i].Trim();
                    if (!String.IsNullOrEmpty(item)) {
                        if (item == "*") {
                            result.IgnoreAll = true;
                            break;
                        }
                        bool endsWith = item.StartsWith("*");
                        bool startsWith = item.EndsWith("*");
                        if (startsWith && endsWith) {
                            result.IgnoreNamesContains.Add(item.Trim('*'));
                        }
                        else if (startsWith && !endsWith) {
                            result.IgnoreNamesStartsWith.Add(item.Trim('*'));
                        }
                        else if (!startsWith && endsWith) {
                            result.IgnoreNamesEndsWith.Add(item.Trim('*'));
                        }
                        else {
                            if (!result.IgnoreNamesExact.Contains(item))
                                result.IgnoreNamesExact.Add(item);
                        }
                    }
                }

                return result;
            }
            catch {
                return result;
            }
        }
    }
}