package com.devexpress.logify.alert.android.collectors;

import android.app.ActivityManager;
import android.app.ActivityManager.MemoryInfo;
import android.content.Context;
import android.content.pm.FeatureInfo;
import android.content.pm.PackageManager;
import android.os.Build;

import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.logger.ILogger;

import java.util.ArrayList;

public class AndroidDeviceCollector implements IInfoCollector {
    private Context context;

    public AndroidDeviceCollector(Context context){
        this.context = context;
    }

    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteObject("device");
        try {
            logger.writeValue("model", Build.MODEL);
            logger.writeValue("brand", Build.BRAND);
            logger.writeValue("manufacturer", Build.MANUFACTURER);
            logger.writeValue("supportedABIs", getSupportedABIs());

            MemoryInfo memoryInfo = getMemoryInfo();
            if (memoryInfo != null) {
                logger.writeValue("totalMemory", Long.toString(memoryInfo.totalMem / 0x100000L) + " Mb");
                logger.writeValue("availableMemory", Long.toString(memoryInfo.availMem / 0x100000L) + " Mb");
                logger.writeValue("lowMemoryThreshold", Long.toString(memoryInfo.threshold / 0x100000L) + " Mb");
                logger.writeValue("isLowMemory", memoryInfo.lowMemory);
            }

            logger.writeValue("features", getFeatureNames());
        } finally {
            logger.endWriteObject("device");
        }
    }

    private String getSupportedABIs() {
        //Build.SUPPORTED_ABIS added in sdk 21
        if (android.os.Build.VERSION.SDK_INT >= 21) {
            StringBuilder supportedABIs = new StringBuilder();
            for (int i = 0; i < Build.SUPPORTED_ABIS.length; i++) {
                if (i > 0)
                    supportedABIs.append(", ");
                supportedABIs.append(Build.SUPPORTED_ABIS[i]);
            }
            return supportedABIs.toString();
        } else
            return null;
    }

    private MemoryInfo getMemoryInfo() {
        if (this.context != null) {
            ActivityManager activityManager = (ActivityManager) this.context.getSystemService(Context.ACTIVITY_SERVICE);
            if (activityManager != null) {
                MemoryInfo memoryInfo = new MemoryInfo();
                activityManager.getMemoryInfo(memoryInfo);
                return memoryInfo;
            }
        }
        return null;
    }

    private ArrayList<String> getFeatureNames() {
        ArrayList<String> featureNames = new ArrayList<String>();
        if (this.context != null) {
            final PackageManager packageManager = this.context.getPackageManager();
            final FeatureInfo[] features = packageManager.getSystemAvailableFeatures();
            for (final FeatureInfo feature : features) {
                if (feature.name != null)
                    featureNames.add(feature.name);
            }
        }
        return featureNames;
    }
}
