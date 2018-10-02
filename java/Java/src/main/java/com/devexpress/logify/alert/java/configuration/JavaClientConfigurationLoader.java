package com.devexpress.logify.alert.java.configuration;

import com.devexpress.logify.alert.core.configuration.ClientConfigurationLoader;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

public class JavaClientConfigurationLoader extends ClientConfigurationLoader {
    @Override
    protected Properties getProperties() {
        Properties properties = new Properties();
        File logifyConfigFile = new File(CONFIGURATION_FILE_NAME);

        if (!logifyConfigFile.exists() || !logifyConfigFile.isFile() || !logifyConfigFile.canRead())
            return null;

        InputStream inputStream = null;
        try {
            inputStream = new FileInputStream(logifyConfigFile.getAbsolutePath());
            properties.load(inputStream);
        } catch (IOException ignored) {
            return null;
        } finally {
            try {
                if (inputStream != null)
                    inputStream.close();
            } catch (IOException ignored) {
            }
        }
        return properties;
    }
}
