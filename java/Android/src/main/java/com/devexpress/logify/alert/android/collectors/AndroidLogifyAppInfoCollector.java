package com.devexpress.logify.alert.android.collectors;

import android.content.Context;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;

import com.devexpress.logify.alert.core.ApplicationProperties;
import com.devexpress.logify.alert.core.collectors.LogifyAppInfoCollector;

public class AndroidLogifyAppInfoCollector extends LogifyAppInfoCollector {
    private Context context;

    public AndroidLogifyAppInfoCollector(Context context, ApplicationProperties applicationProperties){
        super(applicationProperties);
        this.context = context;
    }

    @Override
    protected String getAppName(){
        if(this.getApplicationProperties().AppName != null)
            return this.getApplicationProperties().AppName;
        if(this.context == null)
            return null;
        PackageManager packageManager = context.getPackageManager();
        ApplicationInfo applicationInfo = null;
        try {
            applicationInfo = packageManager.getApplicationInfo(context.getApplicationInfo().packageName, 0);
        } catch (final PackageManager.NameNotFoundException ignored) {
        }
        return (String) (applicationInfo != null ? packageManager.getApplicationLabel(applicationInfo) : context.getApplicationInfo().packageName);
    }

    @Override
    protected String getAppVersion(){
        if (this.getApplicationProperties().AppVersion != null)
            return  this.getApplicationProperties().AppVersion;
        if (this.context == null)
            return null;
        PackageManager packageManager = context.getPackageManager();
        PackageInfo packageInfo = null;
        try {
            packageInfo = packageManager.getPackageInfo(context.getApplicationInfo().packageName, 0);
        } catch (final PackageManager.NameNotFoundException ignored) {
        }
        return (packageInfo != null ? packageInfo.versionName : null);
    }
}
