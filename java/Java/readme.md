# Logify Alert for Java applications
A Java client to report exceptions to [Logify Alert](https://logify.devexpress.com).

## Install
### Maven
In the *pom.xml* file, do the following:
1. In the *dependencies* section, declare a dependency on the *logify-alert-java* library:
    ```xml
    <dependency>
        <groupId>com.devexpress.logify</groupId>
        <artifactId>logify-alert-java</artifactId>
        <version>1.0.2</version>
    </dependency>
    ```
2. In the *repositories* section, declare the Logify Alert's maven repository:
   ```xml
    <repository>
        <id>any-name-or-id</id>
        <url>https://github.com/DevExpress/Logify.Alert.Clients/raw/maven</url>
    </repository>
    ```

### Gradle
In the *build.gradle* file, do the following:
1. In the *dependencies* section, declare a dependency on the *logify-alert-java* library:
    ```jsp
    compile "com.devexpress.logify:logify-alert-java:1.0.2"
    ```
2. In the *repositories* section, declare the Logify Alert's maven repository:
   ```jsp
    maven {
        url "https://github.com/DevExpress/Logify.Alert.Clients/raw/maven"
    }
    ```

## Quick Start
### Send Reports Automatically
```jsp
import com.devexpress.logify.alert.java.LogifyAlert;

 public class MainActivity extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // Initialize the Logify Alert client.
        LogifyAlert client = LogifyAlert.getInstance();
        
        // Specify an API Key to register your application in the Logify Alert service.        
        client.setApiKey("YOUR_API_KEY");

        // Report unhandled exceptions to Logify Alert.
        client.startExceptionsHandling();
    }
}
```

### Send Reports Manually
```jsp
import com.devexpress.logify.alert.java.LogifyAlert;

try {
    RunYourCode();
}
catch (Throwable ex){
    LogifyAlert client = LogifyAlert.getInstance();
    client.setApiKey("YOUR_API_KEY");
    client.send(e);
}
```

## Configuration
You can set up the Logify Alert client in the *logify.properties* file as follows:
```jsp
apiKey=YOUR_API_KEY
appName=YOUR_APP_NAME
appVersion=YOUR_APP_VERSION
userId=YOUR_USER_ID
customData=NAME_1:VALUE_1,NAME_2:VALUE_2,NAME_3:VALUE_3
tags=TAG_1:TAG_VALUE_1,TAG_2:TAG_VALUE_2
```

## API
The *LogifyAlert* class represents the client and provides methods to do the following:
- [Set Up a Client](#Set-Up-a-Client)
- [Add Custom Text to Reports](#Add-Custom-Text-to-Reports)
- [Attach Files to Reports](#Attach-Files-to-Reports)
- [Add Tags to Reports](#Add-Tags-to-Reports)
- [Send Reports Automatically](#Send-Reports-Automatically)
- [Send Reports Manually](#Send-Reports-Manually)
- [Handle Events](#Handle-Events)

### Set Up a Client
- #### getInstance()
    Initializes a new LogifyAlert class instance.
    ```jsp
    LogifyAlert client = LogifyAlert.getInstance();
    ```

- #### getApiKey()
    Returns an [API Key](https://logify.devexpress.com/Alert/Documentation/CreateApp) used to register an application in the Logify Alert service.

- #### getAppName()
    Returns an application's name.

- #### getAppVersion()
    Returns an application's version.

- #### getUserId()
    Returns a unique user ID that corresponds to a sent report.

- #### setApiKey(String apiKey)
    Sets an [API Key](https://logify.devexpress.com/Alert/Documentation/CreateApp) used to register an application in the Logify Alert service.
    ```jsp
    client.setApiKey("YOUR_API_KEY");
    ```

- #### setAppName(String appName)
    Sets an application's name.
    ```jsp
    client.setAppName("My Application");
    ```

- #### setAppVersion(String appVersion)
    Sets an application's version.
    ```jsp
    client.setAppVersion("0.0.0.1");
    ```

- #### setUserId
    Sets a unique user ID that corresponds to a sent report.
    ```jsp
    client.setUserId("A unique user identifier");
    ```

### Add Custom Text to Reports
The Logify Alert client provides methods to attach additional text (for example, a username, computer information, etc.) to generated reports. 
A field name can only consist of a-z, A-Z, 0-9, and _ characters.
- #### addCustomData(String name, String value)
    Adds custom text to a report.
    ```jsp
    client.addCustomData("CustomerName", "Mary");
    ```
- #### getCustomData()
    Returns a collection of custom data sent with a report.
    ```jsp
    client.getCustomData().put("CustomerName", "Mary");
    ```

### Attach Files to Reports
- #### getAttachments()
    Returns a collection of files attached to a report. The total size of attachments is limited to **3 Mb** per crash report. Each attachment should have a unique name.
    ```jsp
    import com.devexpress.logify.alert.java.LogifyAlert;
    import com.devexpress.logify.alert.core.Attachment;
    import java.io.File;
    import java.io.FileInputStream;
    import java.io.IOException;

    LogifyAlert client = LogifyAlert.getInstance();
    client.setApiKey("YOUR_API_KEY");

    File file = new File("C:\\Work\\Image_to_attach.jpg");
    int fileSize = (int) file.length();
    byte bytes[] = new byte[fileSize];
    try {
        FileInputStream inputStream = new FileInputStream(file);
        inputStream.read(bytes, 0, fileSize);
        inputStream.close();
    }
    catch (Exception e) {
    }

    client.getAttachments().add(new Attachment("myAttachedImage.jpg", "image/jpeg", bytes));
    ```

### Add Tags to Reports
The Logify Alert client provides methods to create [tags](https://logify.devexpress.com/Alert/Documentation/ProcessReports/Tags) in generated reports. Tags extend the list of predefined fields used for [auto-ignoring](https://logify.devexpress.com/Alert/Documentation/RejectReports/CustomRules), [searching](https://logify.devexpress.com/Alert/Documentation/ProcessReports/WorkWithReportList/Search) or detecting [duplicates](https://logify.devexpress.com/Alert/Documentation/ProcessReports/DuplicateFields). 
A tag name can only consist of a-z, A-Z, 0-9, and _ characters. A new tag is added with **Allow search** enabled.
- ####  addTag(String name, String value)
    Creates a [tag](https://logify.devexpress.com/Alert/Documentation/ProcessReports/Tags) in a report.
    ```jsp
    client.addTag("key1", "value1");
    ```
- ####  getTags()
    Returns a collection of report [tags](https://logify.devexpress.com/Alert/Documentation/ProcessReports/Tags).
    ```jsp
    client.getTags().put("key1", "value1");
    ```

### Send Reports Automatically
Logify Alert allows you to automatically listen to uncaught exceptions and deliver crash reports. For this purpose, use the following methods:
- ####  startExceptionsHandling()
    Commands Logify Alert to report unhandled exceptions.
    ```jsp
    client.startExceptionsHandling();
    ```
- ####  stopExceptionsHandling()
    Commands Logify Alert to stop reporting unhandled exceptions.
    ```jsp
    client.stopExceptionsHandling();
    ```
### Send Reports Manually
Alternatively, Logify Alert allows you to catch required exceptions manually, generate reports based on caught exceptions and send these reports only. For this purpose, use the following methods:
- ####  send(Throwable ex)
    Generates a crash report based on the caught exception and sends this report to the Logify Alert service.
    ```jsp
    try {
      RunCode();
    }
    catch (Throwable e) {
      client.send(e);
    }
    ```
- ####  send(Throwable ex, Map<String, String> additionalCustomData)
    Sends the caught exception with [custom data](https://logify.devexpress.com/Alert/Documentation/Send/CustomData) to the Logify Alert service.
    ```jsp
    try {
      RunCode();
    }
    catch (Throwable e) {
      Map<String, String> data = new HashMap<String, String>();
      data.put("RunCode", "FailedOperation");
      client.send(e, data);
    }
    ```
- ####  send(Throwable ex, Map<String, String> additionalCustomData, AttachmentCollection additionalAttachments)
    Sends the caught exception with [custom data and attachments](https://logify.devexpress.com/Alert/Documentation/Send/CustomData) to the Logify Alert service.
    ```jsp
    try {
        RunCode();
    }
    catch (Throwable e) {
        Map<String, String> data = new HashMap<String, String>();
        data.put("RunCode", "FailedOperation");
        File sdcard = Environment.getExternalStorageDirectory();
        File file = new File(sdcard, "Download/Image_to_attach.png");
        int size = (int) file.length();
        byte bytes[] = new byte[size];
        try {
            FileInputStream inputStream = new FileInputStream(file);
            inputStream.read(bytes, 0, size);
            inputStream.close();
        }
        catch (IOException ex) {
            ex.printStackTrace();
        }
        AttachmentCollection attachedImages = new AttachmentCollection();
        attachedImages.add(new Attachment("myAttachedImage.png", "image/png", bytes));
        client.send(e, data, attachedImages);
    }
    ```
### Handle Events

- ####  addAfterReportExceptionListener(LogifyEventListener<AfterReportExceptionLogifyEvent> LogifyEventListener) 
    Registers a listener to handle events which occur after a new crash report is sent to the Logify Alert service.
    ```jsp
    import com.devexpress.logify.alert.java.LogifyAlert;
    import com.devexpress.logify.alert.core.events.AfterReportExceptionLogifyEvent;
    import com.devexpress.logify.alert.core.events.LogifyEventListener;
                            
    // Register an event listener with the implementation below.
    LogifyAlert.getInstance().addAfterReportExceptionListener(new AfterReportExceptionEventListener());
    // ...
    // Implement an event listner.
    class AfterReportExceptionEventListener implements LogifyEventListener {
        public void handle(AfterReportExceptionLogifyEvent event) {
            RunYourCode();
        }
    }
    ```
- ####  addBeforeReportExceptionListener(LogifyEventListener<BeforeReportExceptionLogifyEvent> LogifyEventListener)
    Registers a listener to handle events which occur before a new crash report is sent to the Logify Alert service.

    The *BeforeReportException* event occurs between the *CanReportException* event and sending a new report to the Logify Alert service. The *BeforeReportException* event is not raised if you prevent a report from being sent in the *CanReportException* event's handler.

    Handle the *BeforeReportException* event to add custom data to reports. To do this, pass the required data to the [addCustomData](#addCustomData\(String-name\,-String-value\)) method.
    ```jsp
    import com.devexpress.logify.alert.java.LogifyAlert;
    import com.devexpress.logify.alert.core.events.BeforeReportExceptionLogifyEvent;
    import com.devexpress.logify.alert.core.events.LogifyEventListener;
                            
    // Register an event listener with the implementation below.
    LogifyAlert.getInstance().addBeforeReportExceptionListener(new BeforeReportExceptionEventListener());
    // ...
    // Implement an event listner.
    private class BeforeReportExceptionEventListener implements LogifyEventListener {
        public void handle(BeforeReportExceptionLogifyEvent event) {
            LogifyAlert.getInstance().addCustomData("LoggedInUser", "Mary");
        }
    }
    ```
- ####  addCanReportExceptionListener(LogifyEventListener<CanReportExceptionLogifyEvent> LogifyEventListener)
    Registers a listener to handle events which occur after a new report is generated. Handle the *CanReportException* event to cancel sending a report. In this case, the generated report is not posted to the service and the *BeforeReportException* is not raised.
    ```jsp
    import com.devexpress.logify.alert.java.LogifyAlert;
    import com.devexpress.logify.alert.core.events.CanReportExceptionLogifyEvent;
    import com.devexpress.logify.alert.core.events.LogifyEventListener;
                            
    // Register an event listener with the implementation below.
    LogifyAlert.getInstance().addCanReportExceptionListener(new CanReportExceptionEventListener());
    // ...
    // Implement an event listner.
    private class CanReportExceptionEventListener implements LogifyEventListener {
        public void handle(CanReportExceptionLogifyEvent event) {
            // ...
            event.setCancel(true);
        }
    }
    ```
- ####  removeAfterReportExceptionListener(LogifyEventListener<AfterReportExceptionLogifyEvent> LogifyEventListener)
    Removes a listener the addAfterReportExceptionListener method registered.
- ####  removeBeforeReportExceptionListener(LogifyEventListener<BeforeReportExceptionLogifyEvent> LogifyEventListener)
    Removes a listener the addBeforeReportExceptionListener method registered.
- ####  removeCanReportExceptionListener(LogifyEventListener<CanReportExceptionLogifyEvent> LogifyEventListener)
    Removes a listener the addCanReportExceptionListener method registered.

## Custom Clients
If the described client is not suitable for you, you can create a custom one. For more information, refer to the [Custom Clients](https://github.com/DevExpress/Logify.Alert.Clients/blob/develop/CustomClients.md) document.
