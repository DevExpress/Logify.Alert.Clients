# Logify Alert for ASP.NET WebForms and MVC applications
A WebForms and MVC client to report exceptions to [Logify Alert](https://logify.devexpress.com)

## Install <a href="https://www.nuget.org/packages/Logify.Alert.Web/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.Web.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.Web.svg" style="max-width:100%;" /></a>
```sh
$ Install-Package Logify.Alert.Web
```

## Quick Start
### Automatic error reporting
#### Modify Application's Web.config File
Add the Logify Alert settings to the application's **Web.config** file. To initialize your application, use the [API Key](https://logify.devexpress.com/Alert/Documentation/CreateApp) generated for it.
```xml
<configuration>
  <configSections>
    <section name="logifyAlert" type="DevExpress.Logify.WebLogifyConfigSection, Logify.Alert.Web"/>
  </configSections>
  <logifyAlert>
    <apiKey value="SPECIFY_YOUR_API_KEY_HERE"/>
  </logifyAlert>
</configuration>
```
Add the Logify.Alert.Web module to the **Modules** section.
```xml
<system.webServer>
  <modules>
    <add name="Logify.Alert.Web" type="DevExpress.Logify.Web.AspExceptionHandler, Logify.Alert.Web" preCondition="managedHandler"/>
  </modules>
</system.webServer>
```
#### Modify Application's WebApiconfig.cs File (WebApi applications only)
Add the following code to the end of the **Register()** method declared in the application's **WebApiconfig.cs** file.
```csharp
public static class WebApiConfig {
    public static void Register(HttpConfiguration config) {
        //...
        config.Filters.Add(new DevExpress.Logify.Web.WebApiExceptionHandler());
    }
}
```

That's it. Now, your application will report unhandled exceptions to the Logify Alert service. To manage and view generated reports, use the [Logify Alert](https://logify.devexpress.com) link.

### Manual error reporting
```csharp
using DevExpress.Logify.Web;
try {
    LogifyAlert.Instance.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
    RunYourCode();
}
catch (Exception e) {
    LogifyAlert.Instance.Send(e);
}
```

### Manual error reporting via [System.Diagnostics.Trace](https://msdn.microsoft.com/en-us/library/system.diagnostics.trace(v=vs.110).aspx)
```csharp
using System.Diagnostics;
using DevExpress.Logify.Web;
try {
    LogifyAlert.Instance.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
    RunYourCode();
}
catch (Exception e) {
    Trace.TraceError("An exception occurred", e);
}
```

You can set up the Logify Alert trace listener using the **Web.config** file as follows.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.diagnostics>
    <trace autoflush="true" indentsize="4">
      <listeners>
        <add name="LogifyAlertTraceListener"  type="DevExpress.Logify.Web.LogifyAlertTraceListener, Logify.Alert.Web" />
      </listeners>
    </trace>
  </system.diagnostics>
</configuration>
```

## Configuration
You can set up the Logify Alert client using the **Web.config** file as follows.
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="logifyAlert" type="DevExpress.Logify.WebLogifyConfigSection, Logify.Alert.Web" />
  </configSections>
  <logifyAlert>
    <apiKey value="My Api Key" />
    <appName value="My Site" />
    <version value="1.0.5" />
    <offlineReportsEnabled value="true" />
    <offlineReportsCount value="20" />
    <collectBreadcrumbs value="true" />
    <breadcrumbsMaxCount value="500" />
    <ignoreServerVariables value="*http*" />
    <ignoreCookies value="*" />
    <ignoreFormFields value="password, accept" />
    <ignoreRequestBody value="false" />
    <customData>
      <add key="MACHINE_NAME" value="My Server" />
    </customData>
  </logifyAlert>
</configuration>
```

## API
### Properties
#### ApiKey
String. Specifies the [API Key](https://logify.devexpress.com/Alert/Documentation/CreateApp) used to register the applications within the Logify service.
```csharp
client.ApiKey = "My Api Key";
```

#### AppName
String. Specifies the application name.
```csharp
client.AppName = "My Application";
```
#### AppVersion
String. Specifies the application version.
```csharp
client.AppVersion = "1.0.2";
```

#### AllowRemoteConfiguration
Boolean. Gets or sets whether a Logify Alert client configuration can be specified [remotely](https://logify.devexpress.com/Alert/Documentation/Send/RemoteClientConfiguration). The default value is **false**.
```csharp
client.AllowRemoteConfiguration = true;
```
To load client configuration parameters specified remotely, call the *LoadRemoteConfiguration* method. You can also use the *RemoteConfigurationFetchInterval* property to set a time interval specifying how often configuration parameters should be loaded from the server.

#### Attachments
AttachmentCollection. Specifies a collection of files attached to a report. The total attachments size must not be more than **3 Mb** per one crash report. The attachment name must be unique within one crash report.

```csharp
using DevExpress.Logify.Core;
using DevExpress.Logify.Web;

LogifyAlert client = LogifyAlert.Instance;
client.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";

Attachment newAt = new Attachment();
newAt.Name = "My attachment's unique name per one report";
newAt.Content = File.ReadAllBytes(@"C:\Work\Image_to_attach.jpg");
// We strongly recommend that you specify the attachment type.
newAt.MimeType = "image/jpeg";
client.Attachments.Add(newAt);
```

#### Breadcrumbs
BreadcrumbCollection. Specifies a collection of manual breadcrumbs attached to a report. The total breadcrumbs size is limited by 1000 instances (or **3 Mb**) per one crash report by default. To change the maximum allowed size of attached breadcrumbs, use the *BreadcrumbsMaxCount* property.
```csharp
using DevExpress.Logify.Core;
using DevExpress.Logify.Web;

LogifyAlert.Instance.Breadcrumbs.Add(new Breadcrumb() { 
  DateTime = DateTime.UtcNow, 
  Event = BreadcrumbEvent.Manual, 
  Message = "A manually added breadcrumb" 
});
```

#### BreadcrumbsMaxCount
Integer. Specifies the maximum allowed number of breadcrumbs attached to one crash report. The default value is 1000 instances (or 3 MB).
```csharp
LogifyAlert.Instance.BreadcrumbsMaxCount = 2000;
```

#### CollectBreadcrumbs
Boolean. Specifies whether automatic breadcrumbs collecting is enabled. The default value is **false**.
The total breadcrumbs size is limited by 1000 instances (or **3 Mb**) per one crash report by default. To change the maximum allowed size of attached breadcrumbs, use the *BreadcrumbsMaxCount* property.
```csharp
LogifyAlert.Instance.CollectBreadcrumbs = true;
```

#### CustomData
IDictionary<String, String>. Gets the collection of custom data sent with generated reports.
Use the **CustomData** property to attach additional information to the generated report. For instance, you can use this property to track additional metrics that are important in terms of your application: CPU usage, environment parameters, and so on. The field name can only consists of a-z, A-Z, 0-9, and _ characters.

```csharp
client.CustomData["CustomerName"] = "Mary";
```

#### Tags
IDictionary<String, String>. Gets the collection of tags specifying additional fields from a raw report, which will be used in auto ignoring, filtering or detecting duplicates. A key is a tag name (a string that consists of a-z, A-Z, 0-9, and _ characters), and a value is a tag value that is saved to a report. A new tag is added with **Allow search** enabled.

```csharp
client.Tags["OS"] = "Win8";
```

#### IgnoreCookies
String. Specifies cookies that should be excluded from a crash report.  
Provide a comma separated list of names to ignore several cookies. Set this property to the asterisk (\*) character to remove all cookies from a Logify Alert report.  Also, use the asterisk (\*) character as a wildcard to substitute any character(s) when specifying a cookie to be ignored. This property is case-insensitive.

```csharp
LogifyAlert.Instance.IgnoreCookies = "*";
```

#### IgnoreFormFields
String. Specifies form fields that should be excluded from a crash report.  
Provide a comma separated list of names to ignore several form fields. Set this property to the asterisk (\*) character to remove all form data from a Logify Alert report.  Also, use the asterisk (\*) character as a wildcard to substitute any character(s) when specifying a form field to be ignored. For example, when you set *IgnoreFormFields ="\*Password\*"*, Logify Alert will ignore all form fields containing the *“password”* in the name. This property is case-insensitive.

```csharp
LogifyAlert.Instance.IgnoreFormFields = "creditcard,password";
```

#### IgnoreHeaders
String. Specifies request headers that should be excluded from a crash report.  
Provide a comma separated list of names to ignore several headers. Set this property to the asterisk (\*) character to remove all headers from a Logify Alert report.  Also, use the asterisk (\*) character as a wildcard to substitute any character(s) when specifying a header to be ignored. This property is case-insensitive.

```csharp
LogifyAlert.Instance.IgnoreHeaders = "*";
```

#### IgnoreServerVariables
String. Specifies server variables that should be excluded from a crash report.  
Provide a comma separated list of names to ignore several server variables. Set this property to the asterisk (\*) character to remove all server variables from a Logify Alert report.  Also, use the asterisk (\*) character as a wildcard to substitute any character(s) when specifying a server variable to be ignored. This property is case-insensitive.

```csharp
LogifyAlert.Instance.IgnoreServerVariables = "HTTP*";
```

#### IgnoreRequestBody
Boolean. Specifies whether raw request body content should be ignored. 

```csharp
LogifyAlert.Instance.IgnoreRequestBody = true;
```

#### Instance
Singleton. Returns the single instance of the LogifyAlert class.
```csharp
LogifyAlert client = LogifyAlert.Instance;
```

#### OfflineReportsCount
Integer. Specifies the number of last reports to be kept once an Internet connection is lost. Reports are only saved when the *OfflineReportsEnabled* property is set to **true**.
```csharp
client.OfflineReportsEnabled = true;
client.OfflineReportsCount = 20; // Keeps the last 20 reports
```

#### OfflineReportsDirectory
String. Specifies a directory path that will be used to store reports once an Internet connection is lost. Reports are only saved when the *OfflineReportsEnabled* property is set to **true**.
```csharp
client.OfflineReportsEnabled = true;
client.OfflineReportsDirectory = "<directory-for-offline-reports>";
```

#### OfflineReportsEnabled
Boolean. Default value is **false**. Specifies whether or not Logify should store the last *OfflineReportsCount* reports once an Internet connection is lost. To send the kept reports once an Internet connection is available, call the *SendOfflineReports* method.
```csharp
client.OfflineReportsEnabled = true;
client.OfflineReportsCount = 20; // Keeps the last 20 reports
client.OfflineReportsDirectory = "<directory-for-offline-reports>";
```

#### ProxyCredentials
ICredentials. Specifies proxy credentials (a user name and a password) to be used by Logify Alert to authenticate within your system proxy server. The use of this property resolves the "*407 Proxy Authentication Required*" proxy error.
```csharp
client.ProxyCredentials = new NetworkCredential("MyProxyUserName", "MyProxyPassword");
```

#### RemoteConfigurationFetchInterval
Specifies a time interval, in minutes, in which client configuration set [remotely](https://logify.devexpress.com/Alert/Documentation/Send/RemoteClientConfiguration) should be automatically loaded from the server. The minimum value is 2.
```csharp
client.RemoteConfigurationFetchInterval = 5;
```

#### UserId
String. Specifies a unique user identifier that corresponds to the sent report.
```csharp
client.UserId = "user@myapp.com";
```

### Methods for automatic reporting
Logify Alert allows you to automatically listen to uncaught exceptions and deliver crash reports. For this purpose, use the methods below.

#### StartExceptionsHandling()
Commands Logify Alert to start listening to uncaught exceptions and sends reports for all processed exceptions.
```csharp
client.StartExceptionsHandling();
```

#### StopExceptionsHandling()
Commands Logify Alert to stop listening to uncaught exceptions.
```csharp
client.StopExceptionsHandling();
```

### Methods for manual reporting
Alternatively, Logify Alert allows you to catch required exceptions manually, generate reports based on caught exceptions and send these reports only. For this purpose, use the methods below.

#### Send(Exception e)
Generates a crash report based on the caught exception and sends this report to the Logify Alert service.
```csharp
try {
    RunCode();
}
catch (Exception e) {
    client.Send(e);
}
```

#### Send(Exception e, IDictionary<String, String> additionalCustomData)
Sends the caught exception with specified custom data to the Logify Alert service.
```csharp
try {
    RunCode();
}
catch (Exception e) {
    var data = new Dictionary<String, String>();
    data["FailedOperation"] = "RunCode";
    client.Send(e, data);
}
```
#### Send(Exception ex, IDictionary<String, String> additionalCustomData, AttachmentCollection additionalAttachments)
Sends the caught exception with specified custom data and attachments to the Logify Alert service.
```csharp
try {
  RunCode();
}
catch (Exception e) {
  var data = new Dictionary<String, String>();
  data["FailedOperation"] = "RunCode";

  Attachment newAt = new Attachment();
  newAt.Name = "My attachment's unique name per one report";
  newAt.Content = File.ReadAllBytes(@"C:\Work\Image_to_attach.jpg");
  // We strongly recommend that you specify the attachment type.
  newAt.MimeType = "image/jpeg";
  AttachmentCollection newCol = new AttachmentCollection();
  newCol.Add(newAt);

  client.Send(e, data, newCol);
}
```

#### SendAsync(Exception e)
Generates a crash report based on the caught exception and sends this report to the Logify Alert service asynchronously.
```csharp
try {
    RunCode();
}
catch (Exception e) {
    client.SendAsync(e);
}
```

#### SendAsync(Exception e, IDictionary<String, String> additionalCustomData)
Sends the caught exception with specified custom data to the Logify Alert service asynchronously.
```csharp
try {
    RunCode();
}
catch (Exception e) {
    var data = new Dictionary<String, String>();
    data["FailedOperation"] = "RunCode";
    client.SendAsync(e, data);
}
```

#### SendAsync(Exception ex, IDictionary<String, String> additionalCustomData, AttachmentCollection additionalAttachments)
Sends the caught exception with specified custom data and attachments to the Logify Alert service asynchronously.
```csharp
try {
  RunCode();
}
catch (Exception e) {
  var data = new Dictionary<String, String>();
  data["FailedOperation"] = "RunCode";

  Attachment newAt = new Attachment();
  newAt.Name = "My attachment's unique name per one report";
  newAt.Content = File.ReadAllBytes(@"C:\Work\Image_to_attach.jpg");
  // We strongly recommend that you specify the attachment type.
  newAt.MimeType = "image/jpeg";
  AttachmentCollection newCol = new AttachmentCollection();
  newCol.Add(newAt);

  client.SendAsync(e, data, newCol);
}
```

#### SendOfflineReports
Sends all reports saved in the *OfflineReportsDirectory* folder to the Logify Alert service.

### Methods for collecting method arguments
When analyzing an exception’s call stack, it can be useful to review arguments of methods that were being executed when an exception occurred. Logify Alert provides the following methods to support this functionality:

#### TrackArguments(Exception ex, object instance, params object[] args)
#### TrackArguments(Exception ex, int frameCount, MethodCallInfo call)
#### TrackArguments(Exception ex, MethodCallInfo call, int skipFrames)
Collects arguments passed to the invoked method. Call *TrackArguments* when handling an exception that occurs during a method execution.

#### ResetTrackArguments()
Removes method argument values which *TrackArguments* collected previously. Call *ResetTrackArguments* before a method execution and after method execution succeeds.

```csharp
public void DoWork(string work) {
    LogifyAlert.Instance.ResetTrackArguments();
    try {
        DoInnerWork(work, 5);
        LogifyAlert.Instance.ResetTrackArguments();
    }
    catch (Exception ex) {
        LogifyAlert.Instance.TrackArguments(ex, work);
        throw;
    }
}
public void DoInnerWork(string innerWork, this, int times) {
    LogifyAlert.Instance.ResetTrackArguments();
    try {
        object o = null;
        o.ToString();
        LogifyAlert.Instance.ResetTrackArguments();
    }
    catch (Exception ex) {
        LogifyAlert.Instance.TrackArguments(ex, this, innerWork, times);
        throw;
    }
}
```
See the [Collect Method Arguments](https://logify.devexpress.com/Alert/Documentation/Send/MethodArguments) document for details.

### Events

#### AfterReportException
Occurs after Logify Alert sends a new crash report to the service.

```csharp
LogifyAlert.Instance.AfterReportException += OnAfterReportException;

void OnAfterReportException(object sender, AfterReportExceptionEventArgs e) {
  MessageBox.Show("A new crash report has been sent to Logify Alert", "Crash report", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
```

#### BeforeReportException
Occurs before Logify Alert sends a new crash report to the service.

The **BeforeReportException** event occurs between the [CanReportException](#canreportexception) event and sending a new report to the Logify Alert service. If report send is canceled in the [CanReportException](#canreportexception) event's handler, the report is not sent and the **BeforeReportException** event isn't raised.

Handle the **BeforeReportException event** to add custom data to the sent report. To do this, assign the required data to the [CustomData](#customdata) property.
```csharp
LogifyAlert.Instance.BeforeReportException += OnBeforeReportException;

void OnBeforeReportException(object sender, BeforeReportExceptionEventArgs e) {
    LogifyAlert.Instance.CustomData["LoggedInUser"] = "Mary";
}
```


#### CanReportException
Occurs between generating a new crash report and raising the [BeforeReportException](#beforereportexception) event.

The **CanReportException** event occurs right after a new report is generated and prepared to be sent to the Logify Alert service. Handle the **CanReportException** event to cancel sending the report. To do this, assign **true** to the appropriate CanReportExceptionEventArgs's **Cancel** property. Thus, the generated report is not posted to the service and the [BeforeReportException](#beforereportexception) isn't raised.
```csharp
LogifyAlert.Instance.CanReportException += OnCanReportException;

void OnCanReportException(object sender, CanReportExceptionEventArgs args) {
    if (args.Exception is MyCustomException)
        args.Cancel = true;
}
```

### Attributes
#### LogifyIgnoreAttribute
Indicates that exceptions thrown at a specific method should not be handled and sent by Logify Alert.

```csharp
[LogifyIgnore(true)]
void ThisMethodShouldNotReportAnyExceptions() {
    RunSomeCode();
}
```

## Custom Clients
If the described client is not suitable for you, you can create a custom one. For more information, refer to the [Custom Clients](https://github.com/DevExpress/Logify.Alert.Clients/blob/develop/CustomClients.md) document.
