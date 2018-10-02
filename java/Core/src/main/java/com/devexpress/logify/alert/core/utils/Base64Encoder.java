package com.devexpress.logify.alert.core.utils;

public class Base64Encoder {
    private static final String CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    private Base64Encoder() { }

    public static String encode(byte[] bytes) {
        StringBuilder builder = new StringBuilder();

        int suffixLength = bytes.length % 3;
        for (int i = 0; i < bytes.length - suffixLength; i += 3) appendTriplet(bytes, i, builder);

        if (suffixLength != 0) {
            byte[] suffix = new byte[3];
            System.arraycopy(bytes, bytes.length - suffixLength, suffix, 0, suffixLength);
            appendTriplet(suffix, 0, builder);

            for (int i = 0; i < 3 - suffixLength; i++) {
                builder.setCharAt(builder.length() - 1 - i, '=');
            }
        }

        return builder.toString();
    }

    private static void appendTriplet(byte[] source, int position, StringBuilder builder) {
        // When byte is >127, it is treated as a negative number
        // (byte & 0xff) converts it to a positive integer with same bits
        int triplet = ((source[position] & 0xff) << 16) + ((source[position + 1] & 0xff) << 8)
                + (source[position + 2] & 0xff);
        builder.append(CHARACTERS.charAt((triplet >> 18) & 63))
                .append(CHARACTERS.charAt((triplet >> 12) & 63))
                .append(CHARACTERS.charAt((triplet >> 6) & 63))
                .append(CHARACTERS.charAt(triplet & 63));
    }
}