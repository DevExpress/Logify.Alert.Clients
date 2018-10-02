package com.devexpress.logify.alert.android.collectors;

import android.content.Context;
import android.content.res.Configuration;
import android.graphics.Point;
import android.support.test.InstrumentationRegistry;
import android.support.test.runner.AndroidJUnit4;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.WindowManager;

import com.devexpress.logify.alert.core.collectors.CollectorTestsBase;
import com.google.gson.JsonObject;

import org.junit.Test;
import org.junit.runner.RunWith;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

@RunWith(AndroidJUnit4.class)
public class AndroidDisplayCollectorTests extends CollectorTestsBase {

    @Test
    public void testDisplayCollector(){
        new AndroidDisplayCollector(InstrumentationRegistry.getTargetContext()).process(null, logger);

        Context context = InstrumentationRegistry.getTargetContext();
        WindowManager windowManager = (WindowManager) context.getSystemService(Context.WINDOW_SERVICE);
        Display display = windowManager.getDefaultDisplay();

        JsonObject displayJson = parseJsonReport(true).getAsJsonObject("display");
        assertNotNull(displayJson);
        assertEquals(display.getDisplayId(), displayJson.get("displayId").getAsInt());
        assertEquals(display.getName(), displayJson.get("name").getAsString());

        Point size = new Point();
        display.getSize(size);
        assertEquals(size.x, displayJson.get("width").getAsInt());
        assertEquals(size.y, displayJson.get("height").getAsInt());

        Point realSize = new Point();
        display.getRealSize(realSize);
        assertEquals(realSize.x, displayJson.get("realWidth").getAsInt());
        assertEquals(realSize.y, displayJson.get("realHeight").getAsInt());

        int displayRotation = display.getRotation();
        assertEquals(String.format("%s°", displayRotation * 90), displayJson.get("rotation").getAsString());

        assertEquals(getOrientation(), displayJson.get("orientation").getAsString());
        assertEquals( Float.toString(display.getRefreshRate()), displayJson.get("refreshRate").getAsString());


        DisplayMetrics displayMetrics = new DisplayMetrics();
        display.getRealMetrics(displayMetrics);
        assertEquals(displayMetrics.densityDpi, displayJson.get("dpi").getAsInt());
    }

    private String getOrientation() {
        Context context = InstrumentationRegistry.getTargetContext();
        if(context != null) {
            int orientation = context.getResources().getConfiguration().orientation;
            if (orientation == Configuration.ORIENTATION_LANDSCAPE)
                return "landscape";
            else if (orientation == Configuration.ORIENTATION_PORTRAIT)
                return "portrait";
        }
        return null;
    }
}