package com.devexpress.logify.alert.java.collectors;

import com.devexpress.logify.alert.core.ApplicationProperties;
import com.devexpress.logify.alert.core.Attachment;
import com.devexpress.logify.alert.core.collectors.AttachmentCollector;
import com.devexpress.logify.alert.core.collectors.CompositeInfoCollector;
import com.devexpress.logify.alert.core.collectors.CurrentCultureCollector;
import com.devexpress.logify.alert.core.collectors.CustomDataCollector;
import com.devexpress.logify.alert.core.collectors.DevelopmentPlatformCollector;
import com.devexpress.logify.alert.core.collectors.ExceptionInfoCollector;
import com.devexpress.logify.alert.core.collectors.JavaEnvironmentCollector;
import com.devexpress.logify.alert.core.collectors.JvmMemoryCollector;
import com.devexpress.logify.alert.core.collectors.LogifyAppInfoCollector;
import com.devexpress.logify.alert.core.collectors.LogifyProtocolVersionCollector;
import com.devexpress.logify.alert.core.collectors.LogifyReportGenerationDateTimeCollector;
import com.devexpress.logify.alert.core.collectors.TagsCollector;
import com.devexpress.logify.alert.core.collectors.ThreadInfoCollector;
import com.devexpress.logify.alert.core.Platform;
import java.util.List;
import java.util.Map;

public class JavaCompositeInfoCollector extends CompositeInfoCollector {
    public JavaCompositeInfoCollector(ApplicationProperties applicationProperties,
                                      Map<String, String> customData,
                                      List<Attachment> attachments,
                                      Map<String, String> tags
    ) {
        collectors.add(new LogifyProtocolVersionCollector());
        collectors.add(new LogifyReportGenerationDateTimeCollector());
        collectors.add(new DevelopmentPlatformCollector(Platform.java));
        collectors.add(new LogifyAppInfoCollector(applicationProperties));
        collectors.add(new JavaMachineInfoCollector());
        collectors.add(new JavaOperatingSystemCollector());
        collectors.add(new JvmMemoryCollector());
        collectors.add(new CurrentCultureCollector());
        collectors.add(new ThreadInfoCollector());
        collectors.add(new JavaEnvironmentCollector());
        collectors.add(new JavaDisplayCollector());
        collectors.add(new CustomDataCollector(customData));
        collectors.add(new AttachmentCollector(attachments));
        collectors.add(new TagsCollector(tags));

        collectors.add(new ExceptionInfoCollector());
    }
}