package com.devexpress.logify.alert.core;

import org.apache.commons.codec.binary.Base64;
import java.nio.charset.Charset;

//support older versions of Apache Commons Codec library
public class Base64EncoderHelper {
    public static String encodeBase64String(byte[] binaryData) {
        return newStringUtf8(Base64.encodeBase64(binaryData, false));
    }

    private static String newStringUtf8(byte[] bytes) {
        return newString(bytes, Charset.forName("UTF-8"));
    }

    private static String newString(byte[] bytes, Charset charset) {
        return bytes == null?null:new String(bytes, charset);
    }

    public static byte[] decodeBase64(String base64String) {
        return Base64.decodeBase64(getBytesUtf8(base64String));
    }

    public static byte[] getBytesUtf8(String string) {
        return getBytes(string, Charset.forName("UTF-8"));
    }

    private static byte[] getBytes(String string, Charset charset) {
        return string == null?null:string.getBytes(charset);
    }
}
