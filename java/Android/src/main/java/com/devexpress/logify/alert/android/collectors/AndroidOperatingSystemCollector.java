package com.devexpress.logify.alert.android.collectors;

import android.os.Build;
import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.logger.ILogger;
import java.io.BufferedReader;
import java.io.File;
import java.io.InputStreamReader;

public class AndroidOperatingSystemCollector implements IInfoCollector {
    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteObject("os");
        try {
            logger.writeValue("platform", "android");
            logger.writeValue("architecture", System.getProperty("os.arch"));
            logger.writeValue("kernelVersion", System.getProperty("os.version"));
            logger.writeValue("version", Build.VERSION.RELEASE);
            logger.writeValue("apiLevel", Build.VERSION.SDK_INT);
            logger.writeValue("buildId", Build.DISPLAY);
            logger.writeValue("buildFingerprint", Build.FINGERPRINT);
            logger.writeValue("rooted", isDeviceRooted());
        } finally {
            logger.endWriteObject("os");
        }
    }

    private Boolean isDeviceRooted() {
        String buildTags = Build.TAGS;
        if (buildTags != null && buildTags.contains("test-keys"))
            return true;

        String[] paths = {
                "/system/app/SuperSU", "/system/app/SuperSU.apk", "/system/app/Superuser", "/system/app/Superuser.apk",
                "/sbin/su", "/system/bin/su", "/system/xbin/su", "/system/xbin/daemonsu", "/data/local/xbin/su",
                "/data/local/bin/su", "/data/local/su", "/system/sd/xbin/su", "/system/bin/failsafe/su", "/su/bin/su",
                "/su/bin"
        };
        for (String path : paths) {
            if (doesFileExist(path))
                return true;
        }

        String[] commands = { "/system/xbin/which su", "/system/bin/which su", "which su" };
        for (String command : commands)
            if (canExecuteCommand(command))
                return true;

        return false;
    }

    private Boolean doesFileExist(String path) {
        try {
            return new File(path).exists();
        } catch (Exception e) {
            return false;
        }
    }

    private Boolean canExecuteCommand(String command) {
        Process process = null;
        try {
            process = Runtime.getRuntime().exec(command);
            BufferedReader in = new BufferedReader(new InputStreamReader(process.getInputStream()));
            return in.readLine() != null;
        } catch (Exception e) {
            return false;
        } finally {
            if (process != null) process.destroy();
        }
    }
}