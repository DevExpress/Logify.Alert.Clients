package com.devexpress.logify.alert.android.collectors;

import com.devexpress.logify.alert.core.collectors.IInfoCollector;
import com.devexpress.logify.alert.core.logger.ILogger;

import android.content.Context;
import android.content.res.Configuration;
import android.graphics.Point;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.WindowManager;

public class AndroidDisplayCollector implements IInfoCollector {

    private Context context;

    public AndroidDisplayCollector(Context context){
        this.context = context;
    }

    public void process(Throwable ex, ILogger logger) {
        logger.beginWriteObject("display");
        try {
            if (this.context != null) {
                WindowManager windowManager = (WindowManager) this.context.getSystemService(Context.WINDOW_SERVICE);
                if (windowManager != null)
                    processDisplay(windowManager.getDefaultDisplay(), logger);
            }
        } finally {
            logger.endWriteObject("display");
        }
    }

    private void processDisplay(Display display, ILogger logger) {
        if (display == null)
            return;

        logger.writeValue("displayId", display.getDisplayId());
        logger.writeValue("name", display.getName());

        Point size = new Point();
        display.getSize(size);
        logger.writeValue("width", size.x);
        logger.writeValue("height", size.y);

        Point realSize = new Point();
        display.getRealSize(realSize);
        logger.writeValue("realWidth", realSize.x);
        logger.writeValue("realHeight", realSize.y);

        int displayRotation = display.getRotation();
        logger.writeValue("rotation", String.format("%s°", displayRotation * 90));

        logger.writeValue("orientation", getOrientation());

        logger.writeValue("refreshRate", Float.toString(display.getRefreshRate()));

        DisplayMetrics displayMetrics = new DisplayMetrics();
        display.getRealMetrics(displayMetrics);
        logger.writeValue("dpi", displayMetrics.densityDpi);
    }

    private String getOrientation() {
        if (this.context != null) {
            int orientation = this.context.getResources().getConfiguration().orientation;
            if (orientation == Configuration.ORIENTATION_LANDSCAPE)
                return "landscape";
            else if (orientation == Configuration.ORIENTATION_PORTRAIT)
                return "portrait";
        }
        return null;
    }
}
