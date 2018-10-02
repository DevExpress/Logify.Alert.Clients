package com.devexpress.logify.alert.core;

import com.devexpress.logify.alert.core.utils.Base64Encoder;
import org.junit.Test;

import static junit.framework.TestCase.assertEquals;

public class Base64EncoderTests {

    @Test
    public void testNoSuffix() {
        byte[] bytes = "Lorem ips".getBytes();
        assertEquals(Base64EncoderHelper.encodeBase64String(bytes), Base64Encoder.encode(bytes));
    }

    @Test
    public void testSuffix1() {
        byte[] bytes = "Lorem ip".getBytes();
        assertEquals(Base64EncoderHelper.encodeBase64String(bytes), Base64Encoder.encode(bytes));
    }

    @Test
    public void testSuffix2() {
        byte[] bytes = "Lorem i".getBytes();
        assertEquals(Base64EncoderHelper.encodeBase64String(bytes), Base64Encoder.encode(bytes));
    }

    @Test
    public void testSmallBytes() {
        byte[] bytes = {1, 2, 3, 4};
        assertEquals(Base64EncoderHelper.encodeBase64String(bytes), Base64Encoder.encode(bytes));
    }

    @Test
    public void testBigBytes() {
        byte[] bytes = {(byte) 0xfa, (byte) 0xfb, (byte) 0xfc, (byte) 0xfd, (byte) 0xfe, (byte) 0xff};
        assertEquals(Base64EncoderHelper.encodeBase64String(bytes), Base64Encoder.encode(bytes));
    }
}
