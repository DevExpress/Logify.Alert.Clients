using System;
using System.Text;

namespace DevExpress.Logify.Core.Internal {
    public static class RequestBodyFilter {
        public static string FilterRequestBody(string requestBody, IgnorePropertiesInfo ignoreFormFields) {
            try {
                if (String.IsNullOrEmpty(requestBody))
                    return String.Empty;
                string[] pairs = requestBody.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                if (pairs == null || pairs.Length <= 0)
                    return String.Empty;

                StringBuilder result = new StringBuilder();
                int count = pairs.Length;
                for (int i = 0; i < count; i++) {
                    if (!ShouldIgnorePair(pairs[i], ignoreFormFields)) {
                        if (result.Length > 0)
                            result.Append('&');
                        result.Append(pairs[i]);
                    }
                }
                return result.ToString();
            }
            catch {
                return requestBody;
            }
        }

        static bool ShouldIgnorePair(string pair, IgnorePropertiesInfo ignoreFormFields) {
            if (String.IsNullOrEmpty(pair))
                return false;
            int index = pair.IndexOf('=');
            if (index < 0)
                return ignoreFormFields.ShouldIgnore(pair);
            else
                return ignoreFormFields.ShouldIgnore(pair.Substring(0, index));
        }
    }
}