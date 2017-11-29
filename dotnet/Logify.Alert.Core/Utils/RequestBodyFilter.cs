using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevExpress.Logify.Core.Internal {
    public static class RequestBodyFilter {
        public static string ValueStripped { get { return "stripped_by_logify_client"; } }

        public static string GetRequestContent(string method, string contentType, Stream contentStream, Dictionary<string, string> ignoredFormFields) {
            if (contentStream == null)
                return null;

            if (String.Compare(method, "GET", StringComparison.InvariantCultureIgnoreCase) == 0)
                return null;
            if (!String.IsNullOrEmpty(contentType) && contentType.IndexOf("application/x-www-form-urlencoded", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return null;

            if (!contentStream.CanRead || !contentStream.CanSeek)
                return null;

            long position = contentStream.Position;
            string content = new StreamReader(contentStream).ReadToEnd();
            contentStream.Seek(position, SeekOrigin.Begin);
            if (!String.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/form-data", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return FilterMultiPartFormDataRequestBody(content, ignoredFormFields);

            return content;
        }

        static string FilterMultiPartFormDataRequestBody(string content, Dictionary<string, string> ignoredFormFields) {
            if (ignoredFormFields != null && ignoredFormFields.Count > 0) {
                foreach (string key in ignoredFormFields.Keys)
                    content = StripFormField(content, key, ignoredFormFields[key]);
            }

            const int maxContntSize = 8192;
            if (content.Length > maxContntSize)
                return content.Substring(0, maxContntSize);
            else
                return content;
        }

        static string StripFormField(string content, string name, string value) {
            string find = String.Format("name=\"{0}\"\r\n\r\n{1}", name, value);
            string replace = String.Format("name=\"{0}\"\r\n\r\n{1}", name, RequestBodyFilter.ValueStripped);
            return content.Replace(find, replace);
        }

        //static string FilterUrlEncodedRequestBody(string requestBody, IgnorePropertiesInfo ignoreFormFields) {
        //    try {
        //        if (String.IsNullOrEmpty(requestBody))
        //            return String.Empty;

        //        if (ignoreFormFields == null || !ignoreFormFields.IsConfigured)
        //            return requestBody;

        //        string[] pairs = requestBody.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
        //        if (pairs == null || pairs.Length <= 0)
        //            return String.Empty;

        //        StringBuilder result = new StringBuilder();
        //        int count = pairs.Length;
        //        for (int i = 0; i < count; i++) {
        //            if (result.Length > 0)
        //                result.Append('&');
        //            string pair = TryFilterUrlEncodedPair(pairs[i], ignoreFormFields);
        //            result.Append(pair);
        //        }
        //        return result.ToString();
        //    }
        //    catch {
        //        return requestBody;
        //    }
        //}

        //static string TryFilterUrlEncodedPair(string pair, IgnorePropertiesInfo ignoreFormFields) {
        //    if (String.IsNullOrEmpty(pair))
        //        return pair;
        //    int index = pair.IndexOf('=');
        //    if (index < 0)
        //        return pair;
        //    else {
        //        string name = pair.Substring(0, index);
        //        if (ignoreFormFields.ShouldIgnore(name))
        //            return name + "=" + RequestBodyFilter.ValueStripped;
        //        else
        //            return pair;
        //    }
        //}
    }
}