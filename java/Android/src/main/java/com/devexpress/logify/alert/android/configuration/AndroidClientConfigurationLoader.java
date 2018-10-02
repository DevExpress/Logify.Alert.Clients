package com.devexpress.logify.alert.android.configuration;

import android.content.Context;
import android.content.res.AssetManager;
import com.devexpress.logify.alert.core.configuration.ClientConfigurationLoader;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

public class AndroidClientConfigurationLoader extends ClientConfigurationLoader {
    private Context context;

    public  AndroidClientConfigurationLoader(Context context){
        this.context = context;
    }

    @Override
    protected Properties getProperties() {
        if (context == null)
            return  null;

        Properties properties = new Properties();
        InputStream inputStream = null;
        try {
            AssetManager assetManager = context.getAssets();
            inputStream = assetManager.open(CONFIGURATION_FILE_NAME);
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
