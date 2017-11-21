# Logify Alert for WPF applications
A WPF client to report exceptions to [Logify Alert](https://logify.devexpress.com)

## Install <a href="https://www.nuget.org/packages/Logify.Alert.Wpf/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.Wpf.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.Wpf.svg" style="max-width:100%;" /></a>
```sh
$ Install-Package Logify.Alert.Wpf
```

## Quick Start
### Automatic error reporting
```csharp
using DevExpress.Logify.WPF;

LogifyAlert client = LogifyAlert.Instance;
client.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
client.StartExceptionsHandling();
```

### Manual error reporting
```csharp
using DevExpress.Logify.WPF;
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
using DevExpress.Logify.WPF;
try {
    LogifyAlert.Instance.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
    RunYourCode();
}
catch (Exception e) {
    Trace.TraceError("An exception occurred", e);
}
```

You can set up the Logify Alert trace listener using the **App.config** file as follows.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.diagnostics>
    <trace autoflush="true" indentsize="4">
      <listeners>
        <add name="LogifyAlertTraceListener"  type="DevExpress.Logify.WPF.LogifyAlertTraceListener, Logify.Alert.Wpf" />
      </listeners>
    </trace>
  </system.diagnostics>
</configuration>
```

## Configuration
You can setup the Logify Alert client using **App.config** file as follows
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="logifyAlert" type="DevExpress.Logify.LogifyConfigSection, Logify.Alert.Wpf" />
  </configSections>
  <logifyAlert>
    <apiKey value="My Api Key" />
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

#### Attachments
AttachmentCollection. Specifies a collection of files attached to a report. The total attachments size must not be more than **3 Mb** per one crash report. The attachment name must be unique within one crash report.
```csharp
using DevExpress.Logify.Core;
using DevExpress.Logify.WPF;

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
using DevExpress.Logify.WPF;

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
LogifyAlert.Instance.StartExceptionsHandling();
```

#### ConfirmSendReport
Boolean. Specifies whether or not a user must confirm sending reports to the Logify Alert service.
By default, the **ConfirmSendReport** property is set to **false** and all uncaught exceptions are automatically processed and sent to the Logify Alert service.
```csharp
client.ConfirmSendReport = true;
```
To customize the confirmation dialog apperance, handle the **ConfirmationDialogShowing** event.

#### CustomData
IDictionary<String, String>. Gets the collection of custom data sent with generated reports.
Use the **CustomData** property to attach additional information to the generated report. For instance, you can use this property to track additional metrics that are important in terms of your application: CPU usage, environment parameters, and so on. The field name can only consists of a-z, A-Z, 0-9, and _ characters.

```csharp
client.CustomData["CustomerName"] = "Mary";
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

### Events

#### AfterReportException
Occurs after Logify Alert sends a new crash report to the service.

```csharp
LogifyAlert.Instance.AfterReportException += OnAfterReportException;

void OnAfterReportException(object sender, EventArgs e) {
  MessageBox.Show("A new crash report has been sent to Logify Alert", "Crash report", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
```

#### BeforeReportException
Occurs before Logify Alert sends a new crash report to the service.

The **BeforeReportException** event occurs between the [CanReportException](#canreportexception) event and sending a new report to the Logify Alert service. If report send is canceled in the [CanReportException](#canreportexception) event's handler, the report is not sent and the **BeforeReportException** event isn't raised.

Handle the **BeforeReportException event** to add custom data to the sent report. To do this, assign the required data to the [CustomData](#customdata) property.
```csharp
LogifyAlert.Instance.BeforeReportException += OnBeforeReportException;

void OnBeforeReportException(object sender, EventArgs e) {
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

#### ConfirmationDialogShowing

If you set the **ConfirmSendReport** property to **true**, the ConfirmationDialogShowing event occurs before showing the confirmation dialog and allows you to customize the confirmation window's apperance.

```csharp
LogifyAlert.Instance.ConfirmationDialogShowing += Instance_ConfirmationDialogShowing;

private void Instance_ConfirmationDialogShowing(object sender, ConfirmationDialogEventArgs e) {
  ConfirmWindow w = new ConfirmWindow();
  w.DataContext = e.Model;

  // Commands Logify to not show the standard conformation dialog
  e.Handled = true;
  // Shows the custom confirmation window
  w.ShowDialog();

  Environment.Exit(1);
}
```

### Attributes
#### LogifyIgnoreAttribute
Indicates that exceptions thrown at specific method should not be handled and sent by Logify Alert.

```csharp
[LogifyIgnore(true)]
void ThisMethodShouldNotReportAnyExceptions() {
    RunSomeCode();
}
```

## Custom Clients
If the described client is not suitable for you, you can create a custom one. For more information, refer to the [Custom Clients](https://github.com/DevExpress/Logify.Alert.Clients/blob/develop/CustomClients.md) document.
