package com.devexpress.logify.alert.java.collectors;

import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.logger.ILogger;

import java.io.IOException;
import java.net.InetAddress;

public class JavaMachineInfoCollector implements IInfoCollector {

    public void process(Throwable ex, ILogger logger) throws IOException {
        logger.writeValue("domainName", InetAddress.getLocalHost().getCanonicalHostName());

    }
}