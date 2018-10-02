package com.devexpress.logify.alert.core.collectors;

import com.devexpress.logify.alert.core.logger.ILogger;
import java.io.PrintWriter;
import java.io.StringWriter;

public class ExceptionInfoCollector implements IInfoCollector {
    @Override
    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteArray("exception");
        try {
            while (ex != null) {
                logger.beginWriteObject("");
                try {
                    writeException(ex, logger);
                } finally {
                    logger.endWriteObject("");
                }

                ex = ex.getCause();
            }
        } finally {
            logger.endWriteArray("exception");
        }
    }

    void writeException(Throwable ex, ILogger logger) {
        logger.writeValue("type", ex.getClass().getCanonicalName());

        String message = ex.getMessage();
        if (message != null)
            logger.writeValue("message", message);

        StringWriter stackTrace = new StringWriter();
        ex.printStackTrace(new PrintWriter(stackTrace));
        logger.writeValue("stackTrace", stackTrace.toString());

        StringBuilder normalizedStackTrace = new StringBuilder();
        for (StackTraceElement frame : ex.getStackTrace())
            normalizedStackTrace.append(frame).append("\n");
        logger.writeValue("normalizedStackTrace", normalizedStackTrace.toString());
    }
}