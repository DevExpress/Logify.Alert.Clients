package com.devexpress.logify.alert.java.collectors;

import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.logger.ILogger;
import java.awt.Dimension;
import java.awt.AWTError;
import java.awt.GraphicsEnvironment;
import java.awt.Toolkit;
import java.io.IOException;

public class JavaDisplayCollector implements IInfoCollector {

    public void process(Throwable ex, ILogger logger) throws IOException {
        try {
            //native code crash happens on toolkit.getScreenSize() without following string
            GraphicsEnvironment.getLocalGraphicsEnvironment();

            Toolkit toolkit = Toolkit.getDefaultToolkit();

            if(toolkit == null)
                return;

            logger.beginWriteObject("display");
            try {
                Dimension screenSize = toolkit.getScreenSize();
                logger.writeValue("width", (int) screenSize.getWidth());
                logger.writeValue("height", (int) screenSize.getHeight());

                logger.writeValue("colorBits", toolkit.getColorModel().getPixelSize());

                int dpi = toolkit.getScreenResolution();
                logger.writeValue("dpiX", dpi);
                logger.writeValue("dpiY", dpi);
            } finally {
                logger.endWriteObject("display");
            }
        } catch (AWTError e) {
            throw new IOException(e);
        }
    }
}