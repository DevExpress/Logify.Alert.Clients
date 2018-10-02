package com.devexpress.logify.alert.android.collectors;

import android.content.Context;

import com.devexpress.logify.alert.core.ApplicationProperties;
import com.devexpress.logify.alert.core.Attachment;
import com.devexpress.logify.alert.core.collectors.AttachmentCollector;
import com.devexpress.logify.alert.core.collectors.CompositeInfoCollector;
import com.devexpress.logify.alert.core.collectors.CurrentCultureCollector;
import com.devexpress.logify.alert.core.collectors.CustomDataCollector;
import com.devexpress.logify.alert.core.collectors.DevelopmentPlatformCollector;
import com.devexpress.logify.alert.core.collectors.ExceptionInfoCollector;
import com.devexpress.logify.alert.core.collectors.LogifyProtocolVersionCollector;
import com.devexpress.logify.alert.core.collectors.LogifyReportGenerationDateTimeCollector;
import com.devexpress.logify.alert.core.collectors.TagsCollector;
import com.devexpress.logify.alert.core.collectors.ThreadInfoCollector;
import com.devexpress.logify.alert.core.Platform;
import java.util.List;
import java.util.Map;

public class AndroidCompositeInfoCollector extends CompositeInfoCollector {
    public AndroidCompositeInfoCollector(Context context, ApplicationProperties applicationProperties, Map<String, String> customData, List<Attachment> attachments, Map<String, String> tags) {
        collectors.add(new LogifyProtocolVersionCollector());
        collectors.add(new LogifyReportGenerationDateTimeCollector());
        collectors.add(new DevelopmentPlatformCollector(Platform.android));
        collectors.add(new AndroidLogifyAppInfoCollector(context, applicationProperties));
        collectors.add(new AndroidOperatingSystemCollector());
        collectors.add(new CurrentCultureCollector());
        collectors.add(new ThreadInfoCollector());
        collectors.add(new AndroidDisplayCollector(context));
        collectors.add(new AndroidDeviceCollector(context));
        collectors.add(new CustomDataCollector(customData));
        collectors.add(new AttachmentCollector(attachments));
        collectors.add(new TagsCollector(tags));
        collectors.add(new ExceptionInfoCollector());
    }
}